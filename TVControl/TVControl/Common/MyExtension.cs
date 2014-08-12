﻿using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;

namespace KinectControl.Common
{
    public static class MyExtension
    {
        public static List<T> LoadListContent<T>(this ContentManager contentManager, string contentFolder)
        {
            DirectoryInfo dir = new DirectoryInfo(contentManager.RootDirectory + "/" + contentFolder);
            if (!dir.Exists)
                throw new DirectoryNotFoundException();
            List<T> result = new List<T>();

            FileInfo[] files = dir.GetFiles("*.*");
            foreach (FileInfo file in files)
            {
                result.Add(contentManager.Load<T>(contentFolder + "/" + file.Name.Split('.')[0]));
            }
            return result;
        }

    }
}
