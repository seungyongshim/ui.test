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
                // "�α���" â ã��
                var window_LogInWindow = retry(() => app.GetAllTopLevelWindows(automation)
                        , w => w.Length != 0, 100)
                    .Where(x => x.AutomationId == "Window_LogInWindow")
                    .DefaultIfEmpty(null)
                    .First();

                // "���" ��ư Ŭ��
                window_LogInWindow.FindFirstChild("buttonServerList")
                    .AsButton()
                    .WaitUntilEnabled(10.Seconds())
                    .Click();

                // "��������Ʈ" â ã��
                var serverListDialogWindow = retry(() => automation.GetDesktop().FindFirstChild("ServerListDialogWindow")
                    , x => x != null, 100);

                // "�߰�" ��ư Ŭ��
                serverListDialogWindow.FindFirstChild("buttonAdd")
                    .AsButton()
                    .WaitUntilEnabled(3.Seconds())
                    .Click();

                // "���� �߰�" â ã��
                var editServerListDialogWindow = retry(() => serverListDialogWindow.FindFirstChild("EditServerListDialogWindow")
                    , x => x != null, 100)?.AsWindow();

                // "���� �̸�" �Է�
                editServerListDialogWindow.FindFirstChild("serverName").WaitUntilClickable(3.Seconds()).Click();
                Keyboard.Type("222");
                Thread.Sleep(50);

                // "Port" �Է�
                editServerListDialogWindow.FindFirstChild("serverPort").WaitUntilClickable(3.Seconds()).Click();
                Keyboard.Type("1525");
                Thread.Sleep(50);

                // "���� �̸�" �Է�
                editServerListDialogWindow.FindFirstChild("serverID").WaitUntilClickable(3.Seconds()).Click();
                Keyboard.Type("dtmgrdb");
                Thread.Sleep(50);

                // "IP" �Է�
                editServerListDialogWindow.FindFirstChild("serverIP").WaitUntilClickable(3.Seconds()).Click();
                Keyboard.Type("192168100191");
                Thread.Sleep(50);

                // "���� �׽�Ʈ" Ŭ��
                editServerListDialogWindow.FindFirstChild("buttonTest").WaitUntilClickable().Click();

                // "��ȭ ����" ã��
                var dialogbox = retry(() => editServerListDialogWindow.FindFirstDescendant(x => x.ByLocalizedControlType("��ȭ ����"))
                    , x => x != null, 100);

                // "���� �׽�Ʈ" ���� Ȯ��
                dialogbox.FindFirstChild("65535").Name.Should().Be("���� �׽�Ʈ�� �Ϸ�Ǿ����ϴ�.");

                // "Ȯ��" Ŭ��
                dialogbox.FindFirstChild("2").WaitUntilClickable().Click();

                // "Ȯ��" Ŭ��
                editServerListDialogWindow.FindFirstChild("buttonOK").WaitUntilClickable().Click();

                // "DataGrid" ���� Ȯ�� �ϱ�

                var dataPanel = serverListDialogWindow.FindFirstChild("gridControlServerList")
                    .FindFirstChild("dataPresenter");

                var dataRows = dataPanel.FindAllChildren().Select(x => x.AsGridRow());

                var dataRow = dataRows.Where(x => x.Cells[0].Patterns.LegacyIAccessible.Pattern.Value.Value == "222").First();

                dataRow.Click();

                // "Ȯ��" ��ư Ŭ��
                serverListDialogWindow.FindFirstChild("buttonClose")
                    .WaitUntilEnabled(3.Seconds())
                    .Click();

                // "����" �޺� �ڽ� Ŭ��
                window_LogInWindow.FindFirstChild("comboBoxServerList")
                    .WaitUntilClickable(3.Seconds())
                    .Click();

                // "222" Ŭ��
                window_LogInWindow.FindFirstDescendant("PART_Content").FindAllChildren()
                    .Where(x => x.FindFirstChild().Name == "222")
                    .First()
                    .WaitUntilEnabled(3.Seconds())
                    .Click();

                // "�н�����" �Է�
                window_LogInWindow.FindFirstChild("passwordBoxEditUserPass").WaitUntilClickable(3.Seconds()).Click();
                Keyboard.Type("1");
                Thread.Sleep(50);

                // "�α���" Ŭ��
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
                // "�α���" â ã��
                var window_LogInWindow = retry(() => app.GetAllTopLevelWindows(automation)
                        , w => w.Length != 0, 100)
                    .Where(x => x.AutomationId == "Window_LogInWindow")
                    .DefaultIfEmpty(null)
                    .First();

                // ��ٸ���
                window_LogInWindow.FindFirstChild("buttonServerList")
                    .AsButton()
                    .WaitUntilEnabled(10.Seconds());

                // "�н�����" �Է�
                window_LogInWindow.FindFirstChild("passwordBoxEditUserPass").WaitUntilClickable(10.Seconds()).Click();
                Keyboard.Type("1");
                Thread.Sleep(50);

                // "�α���" Ŭ��
                window_LogInWindow.FindFirstChild("buttonLogIn").WaitUntilClickable(3.Seconds()).Click();

                // "������ �޴���" â ã��
                var dXRibbonWindow_DataManager2012V01 = retry(() =>
                        automation.GetDesktop().FindFirstChild("DXRibbonWindow_DataManager2012V01")
                    , x => x != null, 40, 1.Seconds());

                // "�˻�" Ŭ��
                retry(() => dXRibbonWindow_DataManager2012V01.FindFirstDescendant("buttonSearch")
                    , x => x != null, 40).WaitUntilClickable(3.Seconds()).Click();
            }
        }
    }
}
