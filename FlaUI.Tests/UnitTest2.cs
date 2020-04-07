using System;
using System.Linq;
using System.Diagnostics;
using FlaUI.UIA3;
using FlaUI.Core.AutomationElements;
using Xunit;
using System.Threading;
using System.Collections.Generic;
using FlaUI.Core.Input;
using static FlaUI.Tests.Ext;
using FluentAssertions;
using FluentAssertions.Extensions;
using FlaUI.Core.Conditions;
using FlaUI.Core.WindowsAPI;
using FlaUI.Core.Definitions;

namespace FlaUI.Tests
{
    public class UnitTest2
    {
        [Fact(DisplayName = "2. �˻� ����, ���� ���� �� �˻��� 4303 ����Ŭ��")]
        public void Test4()
        {
            using (var automation = new UIA3Automation())
            {
                // "������ �޴���" â ã��
                var dXRibbonWindow_DataManager2012V01 = retry(() =>
                        automation.GetDesktop().FindFirstChild("DXRibbonWindow_DataManager2012V01")
                    , x => x != null, 3, 1.Seconds());

                retry(() => dXRibbonWindow_DataManager2012V01.FindFirstDescendant("comboBoxDateType")
                    , x => x != null, 3).WaitUntilClickable(3.Seconds()).AsComboBox().Select("�˻� �ð�");

                // "�˻� ���� �ð�" Ŭ��
                retry(() => dXRibbonWindow_DataManager2012V01.FindFirstDescendant("dateEditStartTime")
                    , x => x != null, 3).WaitUntilClickable(3.Seconds()).Click();
                Keyboard.Type(VirtualKeyShort.HOME);
                Keyboard.Type("2020-01-01 00:00:00");
                Thread.Sleep(50);

                // "�˻� ���� �ð�" Ŭ��
                retry(() => dXRibbonWindow_DataManager2012V01
                        .FindFirstDescendant("dateEditEndTime"), x => x != null, 3)
                    .WaitUntilClickable(3.Seconds())
                    .Click();
                Keyboard.Type(VirtualKeyShort.HOME);
                Keyboard.Type("2020-12-31 23:59:59");
                Thread.Sleep(50);

                // "�˻�" Ŭ��
                retry(() => dXRibbonWindow_DataManager2012V01
                        .FindFirstDescendant("buttonSearch")
                    , x => x != null, 3).WaitUntilClickable(3.Seconds()).Click();

                var gridControlResultList = retry(() => dXRibbonWindow_DataManager2012V01
                            .FindFirstDescendant("gridControlResultList")
                    , x => x != null, 3);

                var dataPresenter = retry(() => gridControlResultList
                        .FindFirstDescendant("dataPresenter")
                    , x => x != null, 3);

                var dataRows = dataPresenter.FindAllChildren().Select(x => x.AsGridRow());

                // scanIndex: 4303 �� ���� Ŭ��
                dataRows.Where(x => x.Cells[0]
                                        .Patterns
                                        .LegacyIAccessible
                                        .Pattern
                                        .Value
                                        .Value == "4303").First().DoubleClick();



            }
        }
    }
}
