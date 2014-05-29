namespace Reaper.SharpBattleNet.Framework.Extensions
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Reflection;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Runtime;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static class AssemblyExtensions
    {
        public string GetAssemblyTitle(this Assembly assembly)
        {
            var query = from titles in assembly.GetCustomAttributes<AssemblyTitleAttribute>()
                        select titles;

            if(1 == query.Count())
            {
                return query.First().Title;
            }

            return "";
        }

        public string GetAssemblyDescription(this Assembly assembly)
        {
            var query = from descriptions in assembly.GetCustomAttributes<AssemblyDescriptionAttribute>()
                        select descriptions;

            if(1 == query.Count())
            {
                return query.First().Description;
            }

            return "";
        }

        public string GetAssemblyConfiguration(this Assembly assembly)
        {
            var query = from configurations in assembly.GetCustomAttributes<AssemblyConfigurationAttribute>()
                        select configurations;

            if(1 == query.Count())
            {
                return query.First().Configuration;
            }

            return "";
        }

        public string GetAssemblyCompany(this Assembly assembly)
        {
            var query = from companies in assembly.GetCustomAttributes<AssemblyCompanyAttribute>()
                        select companies;

            if(1 == query.Count())
            {
                return query.First().Company;
            }

            return "";
        }

        public string GetAssemblyProduct(this Assembly assembly)
        {
            var query = from products in assembly.GetCustomAttributes<AssemblyProductAttribute>()
                        select products;

            if(1 == query.Count())
            {
                return query.First().Product;
            }

            return "";
        }

        public string GetAssemblyCopyright(this Assembly assembly)
        {
            var query = from copyrights in assembly.GetCustomAttributes<AssemblyCopyrightAttribute>()
                        select copyrights;

            if(1 == query.Count())
            {
                return query.First().Copyright;
            }

            return "";
        }

        public string GetAssemblyTrademark(this Assembly assembly)
        {
            var query = from trademarks in assembly.GetCustomAttributes<AssemblyTrademarkAttribute>()
                        select trademarks;

            if(1 == query.Count())
            {
                return query.First().Trademark;
            }

            return "";
        }

        public string GetAssemblyCulture(this Assembly assembly)
        {
            var query = from cultures in assembly.GetCustomAttributes<AssemblyCultureAttribute>()
                        select cultures;

            if(1 == query.Count())
            {
                return query.First().Culture;
            }

            return "";
        }

        public bool IsAssemblyCOMVisible(this Assembly assembly)
        {
            var query = from visibles in assembly.GetCustomAttributes<ComVisibleAttribute>()
                        select visibles;

            if(1 == query.Count())
            {
                return query.First().Value;
            }

            return false;
        }

        public string GetAssemblyGUID(this Assembly assembly)
        {
            var query = from guids in assembly.GetCustomAttributes<GuidAttribute>()
                        select guids;

            if(1 == query.Count())
            {
                return query.First().Value;
            }

            return "";
        }

        public string GetAssemblyVersion(this Assembly assembly)
        {
            var query = from versions in assembly.GetCustomAttributes<AssemblyVersionAttribute>()
                        select versions;

            if(1 == query.Count())
            {
                return query.First().Version;
            }

            return "";
        }

        public string GetAssemblyFileVersion(this Assembly assembly)
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
