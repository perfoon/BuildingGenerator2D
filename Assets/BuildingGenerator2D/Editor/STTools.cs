using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace ST
{
    public class STTools
    {
        public static string GetAssetPath(Texture2D texture)
        {
            if (texture == null)
                return string.Empty;

            return AssetDatabase.GetAssetPath(texture.GetInstanceID());
        }

        public static string GetAssetPath(Object obj)
        {
            if (obj == null)
                return string.Empty;

            return AssetDatabase.GetAssetPath(obj);
        }

        public static bool IsMainAsset(Object obj)
        {
            return AssetDatabase.IsMainAsset(obj);
        }

        public static bool HasSubAssets(Object obj)
        {
            return (AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(obj)).Length > 1);
        }

        public static Object[] GetSubAssets(Object obj)
        {
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(obj));
            List<Object> subAssets = new List<Object>();

            foreach (Object asset in assets)
            {
                if (AssetDatabase.IsSubAsset(asset))
                    subAssets.Add(asset);
            }

            return subAssets.ToArray();
        }

        public static bool IsDirectory(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            return System.IO.Directory.Exists(path);
        }

        public static Object[] GetDirectoryAssets(string path)
        {
            List<Object> assets = new List<Object>();

            // Get the file paths of all the files in the specified directory
            string[] assetPaths = System.IO.Directory.GetFiles(path);

            // Enumerate through the list of files loading the assets they represent
            foreach (string assetPath in assetPaths)
            {
                // Check if it's a meta file
                if (assetPath.Contains(".meta"))
                    continue;

                Object objAsset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));

                if (objAsset != null)
                    assets.Add(objAsset);
            }

            // Return the array of objects
            return assets.ToArray();
        }

        public static Object[] FilterResourcesForAtlasImport(Object[] resources)
        {
            List<Object> tempList = new List<Object>();

            foreach (Object resource in resources)
            {
                string resourcePath = STTools.GetAssetPath(resource);

                // Check if this is a main asset and queue all it's sub assets
                if (STTools.IsMainAsset(resource) && STTools.HasSubAssets(resource))
                {
                    Object[] subAssets = STTools.FilterResourcesForAtlasImport(STTools.GetSubAssets(resource));

                    foreach (Object a in subAssets) tempList.Add(a);
                }
                else if (resource is Texture2D || resource is Sprite)
                {
                    tempList.Add(resource);
                }
                else if (STTools.IsDirectory(resourcePath))
                {
                    Object[] subAssets = STTools.FilterResourcesForAtlasImport(STTools.GetDirectoryAssets(resourcePath));

                    foreach (Object a in subAssets) tempList.Add(a);
                }
            }

            return tempList.ToArray();
        }

        public static Sprite[] FilterSpritesForAtlasImport(Object[] resources)
        {
            List<Sprite> tempList = new List<Sprite>();

            foreach (Object resource in resources)
            {
                string resourcePath = STTools.GetAssetPath(resource);

                // Check if this is a main asset and queue all it's sub assets
                if (STTools.IsMainAsset(resource) && STTools.HasSubAssets(resource))
                {
                    Object[] subAssets = STTools.FilterResourcesForAtlasImport(STTools.GetSubAssets(resource));

                    foreach (Object a in subAssets)
                    {
                        if (a is Sprite)
                            tempList.Add(a as Sprite);
                    }
                }
                else if (resource is Sprite)
                {
                    tempList.Add((Sprite)resource);
                }
                else if (STTools.IsDirectory(resourcePath))
                {
                    Sprite[] subAssets = STTools.FilterSpritesForAtlasImport(STTools.GetDirectoryAssets(resourcePath));

                    foreach (Sprite a in subAssets) tempList.Add(a);
                }
            }

            return tempList.ToArray();
        }

        public static void CreateAsset<T>(string prefabName, string name) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance(prefabName) as T;

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }


            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + name + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
            /*
            if (projectWindow == null) 
            {
                projectWindow = EditorWindow.GetWindow(ProjectWindowType);
            }
            if (projectWindow != null)
            {
                var e = new Event();
                e.keyCode = KeyCode.F2;
                e.type = EventType.KeyDown;
                projectWindow.SendEvent(e);
            }*/
        }

    }
}
