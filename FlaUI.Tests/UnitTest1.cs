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
    public static class Ext
    {
        public static T retry<T>(Func<T> func, Func<T, bool> predicate, int retryCount, TimeSpan? retryInterval = null)
        {
            var retry = retryInterval ?? TimeSpan.FromMilliseconds(100);
            var exceptions = new List<Exception>();

            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    var result = func();
                    if (predicate(result)) return result;
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
                finally
                {
                    Thread.Sleep(retry);
                }
            }

            if (exceptions.Count != 0)
            {
                throw new AggregateException(exceptions);
            }
            else return default(T);
        }
    }



    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var psi = new ProcessStartInfo
            {
                FileName = @"C:\Mirero.DataManager\Mirero.DataManager.exe",
                WorkingDirectory = @"C:\Mirero.DataManager"
            };

            using (var app = FlaUI.Core.Application.Launch(psi))
            using (var automation = new UIA3Automation())
            {
                // "로그인" 창 찾기
                var window_LogInWindow = retry(() => app.GetAllTopLevelWindows(automation)
                        , w => w.Length != 0, 100)
                    .Where(x => x.AutomationId == "Window_LogInWindow")
                    .DefaultIfEmpty(null)
                    .First();

                // "등록" 버튼 클릭
                window_LogInWindow.FindFirstChild("buttonServerList")
                    .AsButton()
                    .WaitUntilEnabled(10.Seconds())
                    .Click();

                // "서버리스트" 창 찾기
                var serverListDialogWindow = retry(() => automation.GetDesktop().FindFirstChild("ServerListDialogWindow")
                    , x => x != null, 100);

                // "추가" 버튼 클릭
                serverListDialogWindow.FindFirstChild("buttonAdd")
                    .AsButton()
                    .WaitUntilEnabled(3.Seconds())
                    .Click();

                // "서버 추가" 창 찾기
                var editServerListDialogWindow = retry(() => serverListDialogWindow.FindFirstChild("EditServerListDialogWindow")
                    , x => x != null, 100)?.AsWindow();

                // "서버 이름" 입력
                editServerListDialogWindow.FindFirstChild("serverName").WaitUntilClickable(3.Seconds()).Click();
                Keyboard.Type("222");
                Thread.Sleep(50);

                // "Port" 입력
                editServerListDialogWindow.FindFirstChild("serverPort").WaitUntilClickable(3.Seconds()).Click();
                Keyboard.Type("1525");
                Thread.Sleep(50);

                // "서비스 이름" 입력
                editServerListDialogWindow.FindFirstChild("serverID").WaitUntilClickable(3.Seconds()).Click();
                Keyboard.Type("dtmgrdb");
                Thread.Sleep(50);

                // "IP" 입력
                editServerListDialogWindow.FindFirstChild("serverIP").WaitUntilClickable(3.Seconds()).Click();
                Keyboard.Type("192168100191");
                Thread.Sleep(50);

                // "연결 테스트" 클릭
                editServerListDialogWindow.FindFirstChild("buttonTest").WaitUntilClickable().Click();

                // "대화 상자" 찾기
                var dialogbox = retry(() => editServerListDialogWindow.FindFirstDescendant(x => x.ByLocalizedControlType("대화 상자"))
                    , x => x != null, 100);

                // "접속 테스트" 문구 확인
                dialogbox.FindFirstChild("65535").Name.Should().Be("접속 테스트가 완료되었습니다.");

                // "확인" 클릭
                dialogbox.FindFirstChild("2").WaitUntilClickable().Click();

                // "확인" 클릭
                editServerListDialogWindow.FindFirstChild("buttonOK").WaitUntilClickable().Click();

                // "DataGrid" 정보 확인 하기

                var dataPanel = serverListDialogWindow.FindFirstChild("gridControlServerList")
                    .FindFirstChild("dataPresenter");

                var dataRows = dataPanel.FindAllChildren().Select(x => x.AsGridRow());

                var dataRow = dataRows.Where(x => x.Cells[0].Patterns.LegacyIAccessible.Pattern.Value.Value == "222").First();

                dataRow.Click();

                // "확인" 버튼 클릭
                serverListDialogWindow.FindFirstChild("buttonClose")
                    .WaitUntilEnabled(3.Seconds())
                    .Click();

                // "서버" 콤보 박스 클릭
                window_LogInWindow.FindFirstChild("comboBoxServerList")
                    .WaitUntilClickable(3.Seconds())
                    .Click();

                // "222" 클릭
                window_LogInWindow.FindFirstDescendant("PART_Content").FindAllChildren()
                    .Where(x => x.FindFirstChild().Name == "222")
                    .First()
                    .WaitUntilEnabled(3.Seconds())
                    .Click();

                // "패스워드" 입력
                window_LogInWindow.FindFirstChild("passwordBoxEditUserPass").WaitUntilClickable(3.Seconds()).Click();
                Keyboard.Type("1");
                Thread.Sleep(50);

                // "로그인" 클릭
                window_LogInWindow.FindFirstChild("buttonLogIn").WaitUntilClickable(3.Seconds()).Click();

                Thread.Sleep(500);

                app.Kill();
            }
        }

        [Fact]
        public void Test2()
        {
            var psi = new ProcessStartInfo
            {
                FileName = @"C:\Mirero.DataManager\Mirero.DataManager.exe",
                WorkingDirectory = @"C:\Mirero.DataManager"
            };

            
            using (var app = FlaUI.Core.Application.Launch(psi))
            using (var automation = new UIA3Automation())
            {
                // "로그인" 창 찾기
                var window_LogInWindow = retry(() => app.GetAllTopLevelWindows(automation)
                        , w => w.Length != 0, 100)
                    .Where(x => x.AutomationId == "Window_LogInWindow")
                    .DefaultIfEmpty(null)
                    .First();

                // 기다리기
                window_LogInWindow.FindFirstChild("buttonServerList")
                    .AsButton()
                    .WaitUntilEnabled(10.Seconds());

                // "패스워드" 입력
                window_LogInWindow.FindFirstChild("passwordBoxEditUserPass").WaitUntilClickable(10.Seconds()).Click();
                Keyboard.Type("1");
                Thread.Sleep(50);

                // "로그인" 클릭
                window_LogInWindow.FindFirstChild("buttonLogIn").WaitUntilClickable(3.Seconds()).Click();

                // "데이터 메니저" 창 찾기
                var dXRibbonWindow_DataManager2012V01 = retry(() =>
                        automation.GetDesktop().FindFirstChild("DXRibbonWindow_DataManager2012V01")
                    , x => x != null, 40, 1.Seconds());

                // "검색" 클릭
                retry(() => dXRibbonWindow_DataManager2012V01.FindFirstDescendant("buttonSearch")
                    , x => x != null, 40).WaitUntilClickable(3.Seconds()).Click();
            }
        }
    }
}
