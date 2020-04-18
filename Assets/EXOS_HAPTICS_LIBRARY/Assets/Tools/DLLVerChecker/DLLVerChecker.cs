using exiii.Extensions;
using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

#pragma warning disable 414

namespace exiii.Unity
{
    public class DLLVerChecker : MonoBehaviour
    {
        [SerializeField]
        DLLInfo[] DLL;

        private void OnValidate()
        {
            if (DLL == null) { return; }

            DLL.Foreach(x => x.UpdateInfo());
        }
    }

    [Serializable]
    class DLLInfo
    {
        [SerializeField, Unchangeable]
        string DLLName;

        [SerializeField]
        string FilePath;

        [SerializeField, Unchangeable]
        bool PathIsValid;

        [SerializeField, Unchangeable]
        string CompanyName;

        [SerializeField, Unchangeable]
        string FileVersion;

        [SerializeField, Unchangeable]
        string FileDescription;

        public void UpdateInfo()
        {
            if (FilePath == null) { return; }

            FilePath = FilePath.Replace("\"", "");

            PathIsValid = File.Exists(FilePath);

            if (!PathIsValid) { return; }

            FileVersionInfo info = FileVersionInfo.GetVersionInfo(FilePath);

            if (info == null) { return; }

            DLLName = info.ProductName;
            CompanyName = info.CompanyName;
            FileVersion = info.FileVersion;
            FileDescription = info.FileDescription;
        }
    }
}

