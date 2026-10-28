using Serilog;

using System.IO;
using System.Windows;
using System.Reflection;
using Exception = System.Exception;
using MessageBox = System.Windows.MessageBox;

using CobimPalette.Common.CobimBase;
using CobimPalette.Common.LogBase;
using CobimPalette.Common.RibbonBase;

using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Autodesk.AutoCAD.ApplicationServices;
using CobimPalette.Settings;

// TODO : AutoCAD API ���� Ŀ�´�Ƽ ����Ʈ �ʿ�� ���� (2024.11.08 minjae)
// ���� URL - https://forums.autodesk.com/t5/net/bd-p/152

// This line is not mandatory, but improves loading performances
// �ʼ� �ƴ����� �ε� ���� ��� �����ִ� ���
[assembly: ExtensionApplication(typeof(CobimPalette.App))]

namespace CobimPalette
{
    // TODO : �ʿ�� �޼��� "Initialize" ���� ���� ���� (2024.11.07 minjae)
    /// <summary>
    /// Revit �ֵ��� ���α׷� Application.cs�� ����� ��� 
    /// �ش� App Ŭ������ AutoCAD ���� ���α׷����� 
    /// �� �� �ν��Ͻ�ȭ�ǰ� ���� �Ⱓ ���� Ȱ�� ���·� ����
    /// �� �� �ʱ�ȭ�� �������� ������ �ش� App Ŭ���� ���� �ʼ� 
    /// </summary>
    public class App : IExtensionApplication
    {
        #region �⺻ �޼ҵ� 

        /// <summary>
        /// �ֵ��� ���α׷�(CobimPalette) ����(�ʱ�ȭ)
        /// Revit �ֵ��� ���α׷� �޼��� OnStartup�� ����� ��� 
        /// </summary>
        public void Initialize()
        {
            // TODO : AppDomain.CurrentDomain �Ӽ� ����Ͽ� ���� ���� ���� �� ���� ���α׷�(AutoCAD 2025 ����) ������(������� + ���� ������) �������� ���� (2024.10.10 minjae)
            // ���� URL - https://learn.microsoft.com/en-us/dotnet/api/system.appdomain.currentdomain?view=net-8.0
            // ���� 2 URL - https://developer-kus.tistory.com/11
            AppDomain currentDomain = AppDomain.CurrentDomain;   // ���� ���� ���� �� ���� ���α׷�(AutoCAD 2025 ����) ������(������� + ������) ��������

            var currentMethod = MethodBase.GetCurrentMethod();   // �α� ��Ͻ� ���� ���� ���� �޼��� ��ġ ���

            try
            {
                // TODO : ���� �޽��� "System.Windows.Markup.XamlParseException: 'Could not load file or assembly 'AnimatedImage.Wpf, Culture=neutral, PublicKeyToken=75a6352eaa886748'. ������ ������ ã�� �� �����ϴ�.'
                //                       FileNotFoundException: Could not load file or assembly 'AnimatedImage.Wpf, Culture=neutral, PublicKeyToken=75a6352eaa886748'.������ ������ ã�� �� �����ϴ�."  
                //        ���� ���� �ľ� ���
                //        AutoCAD 2024 ���� ������ �� ������ �׷��� �߻��� ���� �� ���� ���µ�,
                //        AutoCAD 2025 ���� ���ʹ� DotNet Core�� �ٲ� ��������,
                //        �̷����� ��Ȳ���� ���� ���� ���� �޽����� �� �߻��ϴ� ������ Ȯ�ε�.
                //        �ش� ���� �޼����� ��µǴ� �ֿ� ������ 3rdparty dll(CobimPalette.dll) ������
                //        �� ���� ���α׷�(acad.exe) ����� �ٷ� ���� �ʰ�,
                //        �� ���� ���α׷�(acad.exe) ���� �� ���߿� 3rdparty dll(CobimPalette.dll) ������ �а� �Ǵ� ��Ȳ������
                //        �� ���� ���α׷�(acad.exe) ��ü�� ������ ������ �˻��ϰ� �Ǵ� ��Ȳ�� �߻��ϱ� �����̴�.  
                // ���� ���� ��� ���� URL -https://stackoverflow.com/questions/1373100/how-to-add-folder-to-assembly-search-path-at-runtime-in-net  
                currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromCobimPalette);

                Logger.ConfigureLogger(CobimHelper.LogFileCountLimit, CobimHelper.AssemblyName, CobimHelper.LogDirectoryPath);   // Serilog �α� �ʱ� ���� 

                Log.Information(Logger.GetMethodPath(currentMethod) + "Serilog �α� �ʱ� ���� �Ϸ�");

                ConfigureAppSetting();   // �ֵ��� ���α׷� CobimPalette �⺻ ������ �ʿ��� ��ü ����(���� �Ǵ� �ʱ�ȭ)  

                Log.Information(Logger.GetMethodPath(currentMethod) + $"{CobimHelper.AssemblyName} ���α׷� ����");

                // static Ŭ���� ComponentManager�� ���� ���� �������� �������� �ʴ� ��� 
                if(ComponentManager.Ribbon is null) 
                    ComponentManager.ItemInitialized += ComponentManager_ItemInitialized;   // �̺�Ʈ �޼��� ComponentManager_ItemInitialized ����
                // static Ŭ���� ComponentManager�� ���� ���� �������� �����ϴ� ��� 
                else Ribbon.CreateRibbonControl(ComponentManager.Ribbon);   // ���� �޴� ���


                Application.DocumentManager.DocumentCreated += App_DocumentCreated;
                Application.DocumentManager.DocumentActivated += App_DocumentActivated;
                Application.DocumentManager.DocumentBecameCurrent += App_DocumentBecameCurrent;
            }
            catch(Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);
            }
        }


        /// <summary>
        /// �ֵ��� ���α׷�(CobimPalette) ����(����)
        /// </summary>
        public void Terminate()
        {
            Application.DocumentManager.DocumentCreated -= App_DocumentCreated;
            Application.DocumentManager.DocumentActivated -= App_DocumentActivated;
            Application.DocumentManager.DocumentBecameCurrent -= App_DocumentBecameCurrent;
        }

        #endregion �⺻ �޼ҵ�

        #region ConfigureAppSetting

        /// <summary>
        /// �ֵ��� ���α׷� CobimPalette �⺻ ������ �ʿ��� ��ü ����(���� �Ǵ� �ʱ�ȭ)
        /// </summary>
        private void ConfigureAppSetting()
        {
            var currentMethod = MethodBase.GetCurrentMethod();   // �α� ��Ͻ� ���� ���� ���� �޼��� ��ġ ���

            try
            {
                AppSetting.Default.DirectoryBase.SetCommonDirectoryPath(CobimHelper.CommonDirectoryPath);

                // TODO : �޼��� "AppSetting.Default.DirectoryBase.SetParentDirectoryPath" ȣ�� -> �޼��� Directory.GetParent ȣ��� ���� parentDirectoryPath�� ��� ������ ��Ʈ(�θ�) ������ ���� ��� ������ �߻���.
                //        �Ͽ� �ش� �޼��� "AppSetting.Default.DirectoryBase.SetParentDirectoryPath" ȣ�⹮ �ּ� ó�� �Ϸ� (2024.10.11 minjae)
                // AppSetting.Default.DirectoryBase.SetParentDirectoryPath(CobimHelper.AssemblyFilePath);   // dll ����(CobimPalette.dll)�� ���� �θ� ���� ��� �������� 
                AppSetting.Default.DirectoryBase.SetLogDirectoryPath(CobimHelper.LogDirectoryPath);

                AppSetting.Default.HttpClientBase.CreateHttpClient();   // HttpClient �̱��� ��ü ����

                // TODO : �ʿ�� AutoCAD �ֵ��� ���α׷�(CobimPalette) �α��� ȭ�� ��� �޼��� OpenLogin() ���� ���� (2024.08.20 minjae)
                // AppSetting.Default.LoginBase.OpenLogin();

                AppSetting.Default.WindowBase.CreateWindow();   // Window ȭ�� ��ü ����

                AppSetting.Default.PaletteBase.CreatePalette();   // Palette ȭ�� �̱��� ��ü ����

                // Palette (WebView2 Ž���� ȭ��) ���
                AppSetting.Default.PaletteBase.RegisterPalette(CobimHelper.WebView2ExplorerTitle, CobimHelper.WebView2ExplorerGuId);

                // TODO : �ʿ�� Palette (HubBox ȭ��) ��� �޼��� "RegisterPalette" ��� ���� (2024.12.04 minjae)
                // Palette (HubBox ȭ��) ��� 
                // AppSetting.Default.PaletteBase.RegisterPalette(CobimHelper.HubBoxTitle, CobimHelper.HubBoxGuId);
            }
            catch (Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                throw;   // ���� �߻��� ���� ȣ���� ����ó�� ���� throw
            }
        }

        #endregion ConfigureAppSetting

        #region App_DocumentCreated

        /// <summary>
        /// AutoCAD ���� ���α׷����� ����ڰ� ���� ������ ����(Document)�� ���� �� ���� ����Ǵ� �̺�Ʈ �޼ҵ�
        /// ���� ���� �� ����(Document) ���� �������� ���� 
        /// </summary>
        private void App_DocumentCreated(object sender, DocumentCollectionEventArgs e)
        {
            var currentMethod = MethodBase.GetCurrentMethod();   // �α� ��Ͻ� ���� ���� ���� �޼��� ��ġ ���

            try
            {
                // Document createdDoc = e.Document;   // ���� ���� �� ����(Document) ���� ��������

                // Palette - WebView2 Ž���� ȭ�� ���
                AppSetting.Default.PaletteBase.ShowPalette(CobimHelper.WebView2ExplorerTitle, CobimHelper.WebView2ExplorerGuId);
                // Palette - HubBox ȭ�� ���
                // AppSetting.Default.PaletteBase.ShowPalette(CobimHelper.HubBoxTitle, CobimHelper.HubBoxGuId);
            }
            catch(Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);
            }
        }

        #endregion App_DocumentCreated

        #region App_DocumentActivated

        /// <summary>
        /// AutoCAD ���� ���α׷����� ����ڰ� ������ ������ ����(Document)�� Ȱ��ȭ�� ���� ����Ǵ� �̺�Ʈ �޼ҵ� 
        /// Ȱ��ȭ �� ����(Document) ���� �������� ���� 
        /// </summary>
        private void App_DocumentActivated(object sender, DocumentCollectionEventArgs e)
        {
            var currentMethod = MethodBase.GetCurrentMethod();   // �α� ��Ͻ� ���� ���� ���� �޼��� ��ġ ���

            try
            {
                // Document activatedDoc = e.Document;   // Ȱ��ȭ �� ����(Document) ���� ��������

                // Palette - WebView2 Ž���� ȭ�� ���
                AppSetting.Default.PaletteBase.ShowPalette(CobimHelper.WebView2ExplorerTitle, CobimHelper.WebView2ExplorerGuId);
                // Palette - HubBox ȭ�� ���
                // AppSetting.Default.PaletteBase.ShowPalette(CobimHelper.HubBoxTitle, CobimHelper.HubBoxGuId);
            }
            catch(Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);
            }
        }

        #endregion App_DocumentActivated

        #region App_DocumentBecameCurrent

        /// <summary>
        /// AutoCAD ���� ���α׷����� ������ �����ִ� ����(Document)�� ���� ������ �����ϰ� 
        /// �� ���� ������ �ٸ� ������ ����(�ٸ� ���� ���� ����, ���� ���...)�� ������ ����Ǵ� �̺�Ʈ �޼ҵ�
        /// ����� ���� �� ����(Document) ���� �������� ����
        /// </summary>
        private void App_DocumentBecameCurrent(object sender, DocumentCollectionEventArgs e)
        {
            var currentMethod = MethodBase.GetCurrentMethod();   // �α� ��Ͻ� ���� ���� ���� �޼��� ��ġ ���

            try
            {
                // Document currentDoc = e.Document;   // ����� ���� �� ����(Document) ���� ��������

                // Palette - WebView2 Ž���� ȭ�� ���
                AppSetting.Default.PaletteBase.ShowPalette(CobimHelper.WebView2ExplorerTitle, CobimHelper.WebView2ExplorerGuId);
                // Palette - HubBox ȭ�� ���
                // AppSetting.Default.PaletteBase.ShowPalette(CobimHelper.HubBoxTitle, CobimHelper.HubBoxGuId);
            }
            catch (Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);
            }
        }

        #endregion App_DocumentBecameCurrent

        #region LoadFromCobimPalette

        /// <summary>
        /// �ֵ��� ���α׷� CobimPalette�� ���� 
        /// dll ����(Nuget ��Ű�� ������ dll ���� ����) �о����
        /// </summary>
        private Assembly LoadFromCobimPalette(object sender, ResolveEventArgs args)
        {
            string folderPath = string.Empty;     // �ֵ��� ���α׷� dll ����(CobimPalette.dll) ���� ���� ���� ��� 
            string assemblyPath = string.Empty;   // �ֵ��� ���α׷� dll ����(CobimPalette.dll) ��� 
            Assembly assembly = null;             // �ֵ��� ���α׷� dll ����(CobimPalette.dll)

            var currentMethod = MethodBase.GetCurrentMethod();   // �α� ��Ͻ� ���� ���� ���� �޼��� ��ġ ���

            try
            {
                folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
                // assemblyPath = Path.Combine(folderPath, Assembly.GetExecutingAssembly().GetName().Name + ".dll");
                // if (!File.Exists(assemblyPath)) return null;
                if (false == File.Exists(assemblyPath)) return null;
                assembly = Assembly.LoadFrom(assemblyPath);
            }
            catch (Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);
            }
            return assembly;
            // return loadingAssembly;
        }

        #endregion LoadFromCobimPalette

        #region ComponentManager_ItemInitialized

        /// <summary>
        /// static Ŭ���� ComponentManager�� ���� ���� ������ �ʱ�ȭ �̺�Ʈ �޼��� 
        /// </summary>
        private void ComponentManager_ItemInitialized(object sender, RibbonItemEventArgs e)
        {
            // static Ŭ���� ComponentManager�� ���� ���� �������� �����ϴ� ��� 
            if(ComponentManager.Ribbon is not null)
            {
                Ribbon.CreateRibbonControl(ComponentManager.Ribbon);   // ���� �޴� ���
                ComponentManager.ItemInitialized -= ComponentManager_ItemInitialized;   // �̺�Ʈ �޼��� ComponentManager_ItemInitialized ����
            }
        }

        #endregion ComponentManager_ItemInitialized

        #region Sample

        #endregion Sample
    }

}
