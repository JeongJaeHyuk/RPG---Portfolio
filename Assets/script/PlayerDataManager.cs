using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

// =====================================================================
// PlayerDataManager : 세이브 파일 전체를 관리하는 클래스
// =====================================================================
// [역할]
// 1. 세이브 파일 생성 / 불러오기 / 삭제
// 2. AES 암호화로 파일 보호 (일반 텍스트 편집기로 열어도 해독 불가)
// 3. 현재 선택된 세이브 슬롯 이름을 씬 전환 후에도 유지
//
// [static 메서드를 쓰는 이유]
// 씬이 전환돼도 데이터를 유지해야 하므로 GameObject에 붙이는 방식(MonoBehaviour)
// 대신 클래스 자체에 직접 접근하는 static 방식으로 구현합니다.
// PlayerDataManager.SaveGame() 처럼 어디서든 바로 호출 가능합니다.
//
// [저장 경로]
// Application.persistentDataPath = Unity가 플랫폼별로 지정하는 안전한 저장 폴더
//   Windows : C:/Users/유저명/AppData/LocalLow/회사명/게임명/
//   Android : /data/data/패키지명/files/
// 이 경로 안에 saves/ 폴더를 만들고 각 세이브를 .sav 파일로 저장합니다.
// =====================================================================
public class PlayerDataManager : MonoBehaviour
{
    // ------------------------------------------------------------------
    // AES 암호화 키 설정
    // ------------------------------------------------------------------
    // AES-256 암호화에는 정확히 32바이트(영문 32글자) 키가 필요합니다.
    // IV(초기화 벡터)는 정확히 16바이트(영문 16글자)가 필요합니다.
    // 이 값을 모르면 파일을 열어도 절대 복호화할 수 없습니다.
    // 주의: 나중에 키를 바꾸면 기존 세이브 파일을 전혀 읽을 수 없게 됩니다.
    // ------------------------------------------------------------------
    const string AES_KEY = "ProjectT_SaveKey_32BytesLongKey!"; // 반드시 32글자
    const string AES_IV  = "ProjectT_IV_16B!";                 // 반드시 16글자

    // 세이브 파일이 저장되는 폴더 경로 (게임 실행 시 자동으로 경로가 결정됨)
    static string SaveFolder => Application.persistentDataPath + "/saves/";

    // 세이브 이름으로 파일 전체 경로를 만들어주는 함수
    // 예: "내캐릭터" → "C:/Users/.../saves/내캐릭터.sav"
    static string SavePath(string saveName) => SaveFolder + saveName + ".sav";

    // ------------------------------------------------------------------
    // 현재 선택된 세이브 이름 (씬 전환 후에도 유지되는 static 변수)
    // ------------------------------------------------------------------
    // StartScene에서 세이브를 선택하면 이 변수에 저장해두고,
    // GameScene에서 이 변수를 읽어서 어떤 파일을 로드할지 결정합니다.
    public static string CurrentSaveName { get; private set; }

    // 새로만들기(true) 인지 불러오기(false) 인지 구분하는 플래그
    // GameScene의 초기화 코드에서 이 값을 보고 데이터 적용 여부를 결정합니다.
    public static bool IsNewGame { get; private set; }

    // ------------------------------------------------------------------
    // 저장 폴더가 없으면 자동 생성
    // ------------------------------------------------------------------
    // 게임을 처음 실행하면 saves/ 폴더가 없으므로 파일 접근 전에 항상 호출합니다.
    static void EnsureSaveFolder()
    {
        if (!Directory.Exists(SaveFolder))
            Directory.CreateDirectory(SaveFolder);
    }

    // ------------------------------------------------------------------
    // 저장된 세이브 파일이 하나라도 있는지 확인
    // StartMenu에서 [불러오기] 버튼 활성화 여부를 결정할 때 사용합니다.
    // ------------------------------------------------------------------
    public static bool HasAnySave()
    {
        EnsureSaveFolder();
        return Directory.GetFiles(SaveFolder, "*.sav").Length > 0;
    }

    // ------------------------------------------------------------------
    // 같은 이름의 세이브 파일이 이미 있는지 확인
    // 새로만들기 시 이름 중복 방지용으로 사용합니다.
    // ------------------------------------------------------------------
    public static bool SaveExists(string saveName)
    {
        return File.Exists(SavePath(saveName));
    }

    // ------------------------------------------------------------------
    // 모든 세이브 슬롯 목록을 최신 저장 순으로 반환
    // StartMenu [불러오기] 패널에서 슬롯 목록을 표시할 때 사용합니다.
    // ------------------------------------------------------------------
    public static List<SaveSlotData> GetAllSaves()
    {
        EnsureSaveFolder();
        List<SaveSlotData> list = new List<SaveSlotData>();

        // saves/ 폴더 안의 모든 .sav 파일을 순회
        string[] files = Directory.GetFiles(SaveFolder, "*.sav");
        foreach (string file in files)
        {
            // 암호화된 파일을 읽고 복호화 → JSON 문자열 → SaveSlotData 객체로 변환
            SaveSlotData data = ReadSaveFile(file);
            if (data != null)
                list.Add(data);
        }

        // saveDate 기준으로 최신 저장 순 정렬 (yyyy-MM-dd HH:mm:ss 형식은 문자열 정렬로 날짜 비교 가능)
        list.Sort((a, b) => string.Compare(b.saveDate, a.saveDate, StringComparison.Ordinal));
        return list;
    }

    // ------------------------------------------------------------------
    // 새 세이브 파일 생성 (새로만들기 확인 버튼에서 호출)
    // ------------------------------------------------------------------
    // 이름 중복 시 false 반환, 성공 시 true 반환
    public static bool CreateSave(string saveName)
    {
        EnsureSaveFolder();

        if (SaveExists(saveName))
            return false; // 이름 중복

        // 새 세이브 데이터 초기값 설정
        SaveSlotData data = new SaveSlotData
        {
            saveName    = saveName,
            saveDate    = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            playerLevel = 1,
            // 나머지 게임 데이터는 기본값(0, null 등)으로 초기화됨
        };

        WriteSaveFile(saveName, data); // 암호화해서 파일로 저장

        CurrentSaveName = saveName;
        IsNewGame = true; // GameScene에서 데이터 로드를 건너뛰도록 표시
        return true;
    }

    // ------------------------------------------------------------------
    // 기존 세이브를 현재 세이브로 선택 (불러오기 슬롯 클릭 시 호출)
    // ------------------------------------------------------------------
    // GameScene 로드 후 LoadGame()을 호출하면 이 이름의 파일을 읽어옵니다.
    public static void SelectSave(string saveName)
    {
        CurrentSaveName = saveName;
        IsNewGame = false; // GameScene에서 데이터를 로드하도록 표시
    }

    // ------------------------------------------------------------------
    // 현재 게임 데이터를 세이브 파일에 저장 (게임씬 내 저장 버튼 또는 자동저장에서 호출)
    // ------------------------------------------------------------------
    // 호출 전에 반드시 각 스크립트(PlayerSpecs, PlayerProgression 등)에서
    // 최신 값을 data에 채워넣어야 합니다.
    public static void SaveGame(SaveSlotData data)
    {
        if (string.IsNullOrEmpty(CurrentSaveName))
        {
            Debug.LogError("저장할 세이브 이름이 없습니다. CreateSave 또는 SelectSave를 먼저 호출하세요.");
            return;
        }

        // 저장 시각과 레벨을 최신으로 갱신
        data.saveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        data.saveName = CurrentSaveName;

        WriteSaveFile(CurrentSaveName, data);
        Debug.Log($"[저장 완료] {CurrentSaveName} : {data.saveDate}");
    }

    // ------------------------------------------------------------------
    // 세이브 파일을 읽어서 게임 데이터를 반환 (GameScene 로드 후 호출)
    // ------------------------------------------------------------------
    // 반환된 SaveSlotData를 각 스크립트에 적용하는 것은 호출한 쪽에서 처리합니다.
    // (PlayerSpecs, PlayerProgression, UI_Inventory 등에 값을 넣어주는 코드 필요)
    public static SaveSlotData LoadGame()
    {
        if (string.IsNullOrEmpty(CurrentSaveName))
        {
            Debug.LogError("불러올 세이브 이름이 없습니다.");
            return null;
        }

        SaveSlotData data = ReadSaveFile(SavePath(CurrentSaveName));
        if (data == null)
        {
            Debug.LogError($"세이브 파일을 읽을 수 없습니다: {CurrentSaveName}");
            return null;
        }

        Debug.Log($"[불러오기 완료] {CurrentSaveName} : {data.saveDate}");
        return data;
    }

    // ------------------------------------------------------------------
    // 세이브 파일 삭제 (불러오기 슬롯의 삭제 버튼에서 호출)
    // ------------------------------------------------------------------
    public static void DeleteSave(string saveName)
    {
        string path = SavePath(saveName);
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"[세이브 삭제] {saveName}");
        }
    }

    // ==================================================================
    // 내부 파일 읽기/쓰기 함수 (암호화 포함)
    // ==================================================================

    // SaveSlotData → JSON 문자열 → AES 암호화 → .sav 파일 저장
    static void WriteSaveFile(string saveName, SaveSlotData data)
    {
        string json = JsonUtility.ToJson(data); // 클래스를 JSON 문자열로 변환
        byte[] encrypted = Encrypt(json);       // JSON 문자열을 AES로 암호화
        File.WriteAllBytes(SavePath(saveName), encrypted); // 암호화된 바이트를 파일로 저장
    }

    // .sav 파일 → AES 복호화 → JSON 문자열 → SaveSlotData 반환
    // 파일이 없거나 복호화 실패 시 null 반환
    static SaveSlotData ReadSaveFile(string filePath)
    {
        if (!File.Exists(filePath))
            return null;

        try
        {
            byte[] encrypted = File.ReadAllBytes(filePath); // 파일에서 암호화된 바이트 읽기
            string json = Decrypt(encrypted);               // AES 복호화 → JSON 문자열
            return JsonUtility.FromJson<SaveSlotData>(json);// JSON → SaveSlotData 객체
        }
        catch (Exception e)
        {
            // 파일이 손상됐거나 키가 다른 경우 (변조 감지)
            Debug.LogError($"세이브 파일 읽기 실패 ({filePath}): {e.Message}");
            return null;
        }
    }

    // ==================================================================
    // AES 암호화 / 복호화
    // ==================================================================
    // AES(Advanced Encryption Standard) : 현재 가장 널리 쓰이는 대칭키 암호화 방식
    // 같은 키로 암호화하고 복호화합니다.
    // 256비트(32바이트) 키를 사용하는 AES-256을 적용합니다.
    // ==================================================================

    // 문자열(JSON)을 암호화해서 바이트 배열로 반환
    static byte[] Encrypt(string plainText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(AES_KEY); // 32바이트 키 설정
            aes.IV  = Encoding.UTF8.GetBytes(AES_IV);  // 16바이트 IV 설정

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                    cs.Write(plainBytes, 0, plainBytes.Length);
                } // cs가 닫힐 때 자동으로 최종 블록 처리(FlushFinalBlock) 됨
                return ms.ToArray();
            }
        }
    }

    // 암호화된 바이트 배열을 복호화해서 원래 문자열(JSON)로 반환
    static string Decrypt(byte[] cipherBytes)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(AES_KEY);
            aes.IV  = Encoding.UTF8.GetBytes(AES_IV);

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using (MemoryStream ms = new MemoryStream(cipherBytes))
            using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (StreamReader sr = new StreamReader(cs))
            {
                return sr.ReadToEnd(); // 복호화된 JSON 문자열 반환
            }
        }
    }
}
