/*=====================================================================
  
  This file is part of the Autodesk Vault API Code Samples.

  Copyright (C) Autodesk Inc.  All rights reserved.

THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
PARTICULAR PURPOSE.
=====================================================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thunderdome
{
    /// <summary>
    /// A set of utilities for manipulating the Vault path
    /// </summary>
    public class VaultPath
    {
        private static string SEPERATOR = "/";

        /// <summary>
        /// Gets the path of the parent folder for the inputed path.
        /// </summary>
        /// <param name="path">A full vault path.</param>
        /// <returns>The parent folder path.</returns>
        public static string GetParentPath(string path)
        {
            string retVal = path.Trim();

            if (retVal.EndsWith(SEPERATOR))
                retVal = retVal.Remove(retVal.Length - 1);

            int index = retVal.LastIndexOf(SEPERATOR);
            if (index < 0)
                return path;
            else
            {
                return retVal.Substring(0, index);
            }
        }

        /// <summary>
        /// Combine 2 Vault path elements into a single path.
        /// </summary>
        /// <param name="path1">First part of the path.</param>
        /// <param name="path2">Second part of the path.</param>
        /// <returns>The combined path.</returns>
        public static string Combine(string path1, string path2)
        {
            path1 = path1.Trim();
            path2 = path2.Trim();

            if (path1.EndsWith(SEPERATOR))
                path1 = path1.Remove(path1.Length - 1);
            if (path2.StartsWith(SEPERATOR))
                path2 = path2.Substring(1);

            return path1 + SEPERATOR + path2;
        }

        /// <summary>
        /// Determines the depth of the path.
        /// </summary>
        /// <param name="path">A full Vault path.</param>
        /// <returns>The number of levels deep that the folder is.  
        /// "$" will have a depth of 0.</returns>
        public static int Depth(string path)
        {
            path = path.Trim();
            if (!path.StartsWith("$"))
                throw new Exception("Intput is not a full path");

            int depth = path.ToCharArray().Count(n => n == '/');

            if (path.EndsWith("/"))
                depth--;

            return depth;
        }
    }
}
