using System;
using System.Collections.Generic;

namespace WebApplication1.Helpers
{
    public static class CategoryIconHelper
    {
        private static readonly Dictionary<string, string> CategoryIcons = new()
        {
            { "laptop", "bi-laptop" },
            { "desktop", "bi-pc-display" },
            { "mobile", "bi-phone" },
            { "tablet", "bi-tablet" },
            { "accessory", "bi-headphones" },
            { "camera", "bi-camera" },
            { "gaming", "bi-controller" },
            { "network", "bi-router" },
            { "printer", "bi-printer" },
            { "software", "bi-window" },
            { "storage", "bi-hdd" },
            { "default", "bi-grid-3x3-gap" }
        };

        public static string GetIconClass(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
                return CategoryIcons["default"];

            foreach (var pair in CategoryIcons)
            {
                if (categoryName.ToLower().Contains(pair.Key))
                    return pair.Value;
            }

            return CategoryIcons["default"];
        }
    }
}
