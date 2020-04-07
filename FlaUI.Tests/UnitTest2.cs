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
        [Fact(DisplayName = "2. 검사 시작, 종료 설정 후 검색후 4303 더블클릭")]
        public void Test4()
        {
            using (var automation = new UIA3Automation())
            {
                // "데이터 메니저" 창 찾기
                var dXRibbonWindow_DataManager2012V01 = retry(() =>
                        automation.GetDesktop().FindFirstChild("DXRibbonWindow_DataManager2012V01")
                    , x => x != null, 3, 1.Seconds());

                retry(() => dXRibbonWindow_DataManager2012V01.FindFirstDescendant("comboBoxDateType")
                    , x => x != null, 3).WaitUntilClickable(3.Seconds()).AsComboBox().Select("검사 시간");

                // "검사 시작 시간" 클릭
                retry(() => dXRibbonWindow_DataManager2012V01.FindFirstDescendant("dateEditStartTime")
                    , x => x != null, 3).WaitUntilClickable(3.Seconds()).Click();
                Keyboard.Type(VirtualKeyShort.HOME);
                Keyboard.Type("2020-01-01 00:00:00");
                Thread.Sleep(50);

                // "검사 종료 시간" 클릭
                retry(() => dXRibbonWindow_DataManager2012V01
                        .FindFirstDescendant("dateEditEndTime"), x => x != null, 3)
                    .WaitUntilClickable(3.Seconds())
                    .Click();
                Keyboard.Type(VirtualKeyShort.HOME);
                Keyboard.Type("2020-12-31 23:59:59");
                Thread.Sleep(50);

                // "검색" 클릭
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

                // scanIndex: 4303 행 더블 클릭
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
