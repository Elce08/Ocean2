using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum Item
{
    None= 0,
    seaglider,
    airtank = 10,
    head = 20,
    body,
    titanium = 101,
    copper,
    coal,
    quartz,
    plastic = 201,
    glass,
    coppercable,
    water = 401,
    fish1,
    fish2,
    fish3,
    fish4,
    cookedFish1,
    cookedFish2,
    cookedFish3,
    cookedFish4,
}

public class ItemManager : MonoBehaviour
{
    public ItemManager another = null;
    public Items[] itemPrefebs;
    Player player;

    GridLayoutGroup group;
    public RectTransform rectTransform;

    public int width;
    public int height;
    public bool equip = false;

    public Sprite[] itemSprites;

    /// <summary>
    /// Change Inven by ItemIndex
    /// </summary>
    public System.Action<int> changeInven;

    /// <summary>
    /// Set item's width and height
    /// </summary>
    /// <param name="item">Check Data target Item</param>
    /// <returns>x : width, y : height</returns>
    public Vector2Int ItemData(Item item)
    {
        Vector2Int result = new();
        switch (item)
        {
            case Item.seaglider:
                result.x = 3;
                result.y = 3;
                break;
            case Item.airtank:
                result.x = 2;
                result.y = 3;
                break;
            case Item.head:
            case Item.body:
                result.x = 2;
                result.y = 2;
                break;
            default:
                result.x = 1;
                result.y = 1;
                break;
        }
        return result;
    }

    public Slots[] slots;

    public List<(Item,List<Slots>)> itemsSlots = new();
    public List<Item> items = new();

    protected virtual void Awake()
    {
        player = FindObjectOfType<Player>();
        group = GetComponent<GridLayoutGroup>();
        slots = GetComponentsInChildren<Slots>();
        rectTransform = GetComponent<RectTransform>();
        width = (int)(rectTransform.sizeDelta.x / group.cellSize.x);
        height = (int)(rectTransform.sizeDelta.y / group.cellSize.y);
    }

    Item lastItem;

    /// <summary>
    /// Add Item
    /// </summary>
    /// <param name="item">Add target item</param>
    /// <returns>if add success or not</returns>
    public virtual bool Add(Item item)
    {
        items.Add(item);
        lastItem = item;
        if(RefreshSlots()) return true;
        else return false;
    }

    public void Remove(Item item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            RefreshSlots();
        }
    }

    /// <summary>
    /// Remove Item
    /// </summary>
    /// <param name="index">Remove target item's index</param>
    public void Remove(int index)
    {
        items.Remove(items[index]);
        RefreshSlots();
    }

    /// <summary>
    /// Give Item to another Inven
    /// </summary>
    /// <param name="index">Give target item's index</param>
    public virtual void ChangeInven(int index)
    {
        if (another != null)
        {
            if (another.Add(items[index]))
            {
                Remove(index);
            }
        }
    }

    /// <summary>
    /// Drop Item
    /// </summary>
    /// <param name="index">Drop target item's index</param>
    public void DropItem(int index)
    {
        foreach(Items dropItem in itemPrefebs)
        {
            if(dropItem.sort == items[index])
            {
                Items tems = Instantiate(dropItem, player.transform.position, Quaternion.identity).GetComponent<Items>();
                tems.inven = this;
            }
        }
        Remove(index);
    }

    public void DropItem(Item item)
    {
        Remove(item);
    }

    /// <summary>
    /// Refresh inventory and sort items
    /// </summary>
    /// <returns>If success or not</returns>
    protected bool RefreshSlots()
    {
        bool result = true;
        items.Sort();
        foreach(Slots slot in slots)
        {
            slot.Engaged = false;
        }
        int index = 0;
        foreach(Item item in items)
        {
            int x = ItemData(item).x;
            int y = ItemData(item).y;
            int start = 0;
            List<Slots> Slots = new();
            foreach(Slots startSlot in slots)
            {
                if (start % width + x > width) start++;
                else if (slots[start+x-1].Engaged) start++;
                else 
                { 
                    if (!startSlot.Engaged)
                    {
                        break;
                    }
                    else start++;
                }
            }
            if(start >= (width * height))
            {
                DropItem(lastItem) ;
                result = false;
                break;
            }
            if((start + (x-1) + (y-1) * width) > (width * height))
            {
                DropItem(lastItem);
                result= false;
                break;
            }

            for(int i = 0; i < x; i++)
            {
                for(int j = 0; j < y; j++)
                {
                    Slots.Add(slots[(start + i + (j * width))]);
                    slots[(start + i + (j * width))].item = item;
                    slots[(start + i + (j * width))].itemIndex = index;
                    slots[(start + i + (j * width))].Engaged = true;
                }
            }
            itemsSlots.Add(new(item, Slots));
            index++;
        }
        return result;
    }
}
