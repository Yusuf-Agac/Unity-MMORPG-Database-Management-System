using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class CreateItem : MonoBehaviour
{
    private DBManager _dbManager;
    public TMP_Dropdown ItemTypeDD;
    public TMP_Dropdown ItemIndexDD;
    private Inventory _inventory;
    private PlayerInfo _playerInfo;

    private void Start()
    {
        _dbManager = GetComponent<DBManager>();
        _inventory = GetComponent<Inventory>();
        _playerInfo = GetComponent<PlayerInfo>();
    }

    public void CreateItemFunc()
    {
        StartCoroutine(CreateItemCo(int.Parse(ItemIndexDD.options[ItemIndexDD.value].text), ItemTypeDD.options[ItemTypeDD.value].text));
    }
    
    public void CreateItemFuncWithParameter(int index, string type)
    {
        StartCoroutine(CreateItemCo(index, type));
    }

    IEnumerator CreateItemCo(int itemIndex, string itemName)
    {
        Debug.Log("ID --> " + _playerInfo.ID);
        WWWForm form = new WWWForm();
        form.AddField("ItemName", itemName);
        form.AddField("ItemIndex", itemIndex);
        form.AddField("ID", _playerInfo.ID.ToString());
        
        UnityWebRequest req = UnityWebRequest.Post("http://localhost/sqlconnect/CreateItem.php", form);
        req.downloadHandler = new DownloadHandlerBuffer();
        yield return req.SendWebRequest();
        
        WWWForm form2 = new WWWForm();
        form2.AddField("ID", _playerInfo.ID);
        form2.AddField("ItemIndex", itemIndex);
        
        UnityWebRequest req2 = UnityWebRequest.Post("http://localhost/sqlconnect/GetItemID.php", form);
        req2.downloadHandler = new DownloadHandlerBuffer();
        
        yield return req2.SendWebRequest();
        
        if (req.downloadHandler.text == "0")
        {
            _inventory.LoadItemToUI(itemIndex, itemName, int.Parse(_playerInfo.ID), int.Parse(req2.downloadHandler.text));
            string[] itemInfo = new string[5];
            itemInfo[0] = req2.downloadHandler.text;
            itemInfo[1] = itemName;
            itemInfo[2] = itemIndex.ToString();
            itemInfo[3] = _playerInfo.ID;
            _inventory.AddToItemsList(itemInfo);
            Debug.Log("Item successfully created");
        }
        else
        {
            Debug.LogWarning("Item creation failed: # " + req.downloadHandler.text);
        }
    }
}
