#region Header
//
//    _  _   ____        _   _   _         _   _      _   
//  _| || |_| __ )  __ _| |_| |_| | ___   | \ | | ___| |_ 
// |_  .. _ |  _ \ / _` | __| __| |/ _ \  |  \| |/ _ \ __|
// |_      _| |_) | (_| | |_| |_| |  __/_ | |\  |  __/ |_ 
//   |_||_| |____/ \__,_|\__|\__|_|\___(_)_ | \_|\___|\__|
//
// The MIT License
// 
// Copyright(c) 2014 Wynand Pieters. https://github.com/wpieterse/SharpBattleNet

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
#endregion

namespace SharpBattleNet.Framework.Utilities.Extensions
{
    #region Usings
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    #endregion

    /// <summary>
    /// Provides extension methods for <see cref="Assembly"/>.
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Returns the assembly title object.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string GetAssemblyTitle(this Assembly assembly)
        {
            var query = from titles in assembly.GetCustomAttributes<AssemblyTitleAttribute>()
                        select titles;

            if(1 == query.Count())
            {
                return query.First().Title;
            }

            return "";
        }

        public static string GetAssemblyDescription(this Assembly assembly)
        {
            var query = from descriptions in assembly.GetCustomAttributes<AssemblyDescriptionAttribute>()
                        select descriptions;

            if(1 == query.Count())
            {
                return query.First().Description;
            }

            return "";
        }

        public static string GetAssemblyConfiguration(this Assembly assembly)
        {
            var query = from configurations in assembly.GetCustomAttributes<AssemblyConfigurationAttribute>()
                        select configurations;

            if(1 == query.Count())
            {
                return query.First().Configuration;
            }

            return "";
        }

        public static string GetAssemblyCompany(this Assembly assembly)
        {
            var query = from companies in assembly.GetCustomAttributes<AssemblyCompanyAttribute>()
                        select companies;

            if(1 == query.Count())
            {
                return query.First().Company;
            }

            return "";
        }

        public static string GetAssemblyProduct(this Assembly assembly)
        {
            var query = from products in assembly.GetCustomAttributes<AssemblyProductAttribute>()
                        select products;

            if(1 == query.Count())
            {
                return query.First().Product;
            }

            return "";
        }

        public static string GetAssemblyCopyright(this Assembly assembly)
        {
            var query = from copyrights in assembly.GetCustomAttributes<AssemblyCopyrightAttribute>()
                        select copyrights;

            if(1 == query.Count())
            {
                return query.First().Copyright;
            }

            return "";
        }

        public static string GetAssemblyTrademark(this Assembly assembly)
        {
            var query = from trademarks in assembly.GetCustomAttributes<AssemblyTrademarkAttribute>()
                        select trademarks;

            if(1 == query.Count())
            {
                return query.First().Trademark;
            }

            return "";
        }

        public static string GetAssemblyCulture(this Assembly assembly)
        {
            var query = from cultures in assembly.GetCustomAttributes<AssemblyCultureAttribute>()
                        select cultures;

            if(1 == query.Count())
            {
                return query.First().Culture;
            }

            return "";
        }

        public static bool IsAssemblyCOMVisible(this Assembly assembly)
        {
            var query = from visibles in assembly.GetCustomAttributes<ComVisibleAttribute>()
                        select visibles;

            if(1 == query.Count())
            {
                return query.First().Value;
            }

            return false;
        }

        public static string GetAssemblyGUID(this Assembly assembly)
        {
            var query = from guids in assembly.GetCustomAttributes<GuidAttribute>()
                        select guids;

            if(1 == query.Count())
            {
                return query.First().Value;
            }

            return "";
        }

        public static string GetAssemblyVersion(this Assembly assembly)
        {
            var query = from versions in assembly.GetCustomAttributes<AssemblyVersionAttribute>()
                        select versions;

            if(1 == query.Count())
            {
                return query.First().Version;
            }

            return "";
        }

        public static string GetAssemblyFileVersion(this Assembly assembly)
        {
            var query = from versions in assembly.GetCustomAttributes<AssemblyFileVersionAttribute>()
                        select versions;

            if(1 == query.Count())
            {
                return query.First().Version;
            }

            return "";
        }
    }
}

