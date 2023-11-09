using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TestInventory : TestBase
{
    public ItemManager inven;

    protected override void Test1(InputAction.CallbackContext _)
    {
        inven.Add(Item.seaglider);
    }

    protected override void Test2(InputAction.CallbackContext _)
    {
        inven.Add(Item.airtank);
    }

    protected override void Test3(InputAction.CallbackContext _)
    {
        inven.Add(Item.head);
    }

    protected override void Test4(InputAction.CallbackContext _)
    {
        inven.Add(Item.water);
    }

    protected override void Test5(InputAction.CallbackContext _)
    {
    }

    protected override void TestRClick(InputAction.CallbackContext _)
    {
    }

}