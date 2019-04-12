﻿namespace com.organo.x4ever.Models
{
    public class FileDetail
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Parent { get; set; }
        public bool IsDirectory { get; set; }
        public bool IsFile { get; set; }
        public string DirectoryOrFile => IsFile ? "File" : "Directory";
        public string Type => GetType();

        private new string GetType()
        {
            if (Name != null)
            {
                var names = Name.Split('.');
                var type = names[names.Length - 1];
                return type.ToUpper();
            }

            return "";
        }
    }
}