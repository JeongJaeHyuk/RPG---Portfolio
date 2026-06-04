using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
public class Item_Data_Manager : MonoBehaviour
{
    // 프로퍼티로 변경 - 매번 호출 시 최신 경로 반환
    string itemDataPath => Application.dataPath + "/Resources/ItemDataPath/ItemData.csv";
    public List<Item> Item_Load_CSV()
    {
        List<Item> itemList = new List<Item>();
        if (File.Exists(itemDataPath))
        {
            string line = string.Empty;
            using (StreamReader sr = new StreamReader(itemDataPath))
            {
                line = sr.ReadLine(); // 첫번쨰줄 필요없는 부분읽고
                while ((line = sr.ReadLine()) != null)
                {
                    string[] data = line.Split(",");
                    if (data.Length >= 12)
                    {
                        int itemId = int.Parse(data[0].Trim());
                        string itemName =  data[1].Trim();
                        string itemTypeName = data[2].Trim();
                        string itemIconName = data[2].Trim();                        
                        string description = data[3].Trim();
                        string typeString = data[4].Trim();
                        string armorTypeString = data[5].Trim();
                        float attackPower = float.Parse(data[6].Trim());
                        float defen = float.Parse(data[7].Trim());
                        float hpRecover = float.Parse(data[8].Trim());
                        float mpRecover = float.Parse(data[9].Trim());
                        int price = int.Parse(data[10].Trim());
                        int maxCount = int.Parse(data[11].Trim());
                        ItemType itemType; // 아이템타입 들어갈 변수
                        if(!Enum.TryParse(typeString, out itemType)) // 첫번째 매개변수의 이름이 itemType에 맞는 이름이 있으면 그값으로 설정
                        {
                            // ItemType 열거체자료형안에 맞는 이름이 없을경우 실행
                            Debug.Log(itemName + "의 아이템타입을 찾을수없어 기본값 Material로 설정합니다");
                            itemType = ItemType.Material; // 기본값으로 설정
                        }
                        ArmorType armorType; // 방어구 타입 들어갈 변수
                        if(!Enum.TryParse(armorTypeString, out armorType))
                        {
                            // ArmorType 열거체자료형안에 맞는 이름이 없을경우 실행
                            Debug.Log(itemName + "의 방어구타입을 찾을수없어 기본값 None으로 설정합니다");
                            armorType = ArmorType.None; // 기본값으로 설정
                        }
                        Item item = new Item
                        {
                        itemId = itemId,
                        itemName = itemName,
                        itemTypeName = itemTypeName,
                        itemIcon = ResourcesLoadManager.rcManager.GetRcItemIcon(itemIconName),
                        description = description,
                        type = itemType,
                        armorType = armorType,
                        attackPower = attackPower,
                        defense = defen,
                        hpRecover = hpRecover,
                        mpRecover = mpRecover,
                        price = price,
                        maxCount = maxCount,
                        currentCount = 0
                        };
                        itemList.Add(item);
                    }
                }

            }
        }
        return itemList;
    }
}
