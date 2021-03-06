﻿//    This file is part of OleViewDotNet.
//    Copyright (C) James Forshaw 2014
//
//    OleViewDotNet is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    OleViewDotNet is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with OleViewDotNet.  If not, see <http://www.gnu.org/licenses/>.

using System;
using Microsoft.Win32;
using OleViewDotNet.InterfaceViewers;

namespace OleViewDotNet
{
    [Serializable]
    public class COMInterfaceEntry : IComparable<COMInterfaceEntry>
    {
        public int CompareTo(COMInterfaceEntry right)
        {
            return String.Compare(Name, right.Name);
        }

        private void LoadFromKey(RegistryKey key)
        {
            object name = key.GetValue(null);
            if ((name != null) && (name.ToString().Length > 0))
            {
                Name = name.ToString();
            }
            else
            {
                Name = String.Format("{{{0}}}", Iid.ToString());
            }         
              
            ProxyClsid = COMUtilities.ReadGuidFromKey(key, "ProxyStubCLSID32", null);
            NumMethods = COMUtilities.ReadIntFromKey(key, "NumMethods", null);
            
            if (NumMethods < 3)
            {
                NumMethods = 3;
            }
            
            TypeLib = COMUtilities.ReadGuidFromKey(key, "TypeLib", null);
            TypeLibVersion = COMUtilities.ReadStringFromKey(key, "TypeLib", "Version");

            Base = COMUtilities.ReadStringFromKey(key, "BaseInterface", null);
            if (Base.Length == 0)
            {
                Base = "IUnknown";
            }
        }

        private COMInterfaceEntry()
        {
        }

        private COMInterfaceEntry(Guid iid, Guid proxyclsid, int nummethods, string baseName, string name)
        {
            Iid = iid;
            ProxyClsid = proxyclsid;
            NumMethods = nummethods;
            Base = baseName;
            Name = name;
        }

        public COMInterfaceEntry(Guid iid, RegistryKey rootKey) 
            : this(iid, Guid.Empty, 3, "IUnknown", "")
        {            
            LoadFromKey(rootKey);
        }

        public COMInterfaceEntry(Guid iid)
            : this(iid, Guid.Empty, 3, "IUnknown", iid.ToString("B"))
        {
        }

        public enum KnownInterfaces
        {
            IUnknown,
            IMarshal
        }

        public static Guid IID_IUnknown
        {
            get { return new Guid("{00000000-0000-0000-C000-000000000046}"); }
        }

        public static Guid IID_IMarshal
        {
            get { return new Guid("{00000003-0000-0000-C000-000000000046}"); }
        }

        public static Guid IID_IDispatch
        {
            get { return new Guid("00020400-0000-0000-c000-000000000046"); }
        }

        public static Guid IID_IOleControl
        {
            get { return new Guid("{b196b288-bab4-101a-b69c-00aa00341d07}"); }
        }

        public static Guid IID_IPersistStream
        {
            get { return typeof(IPersistStream).GUID; }
        }

        public static Guid IID_IPersistStreamInit
        {
            get { return typeof(IPersistStreamInit).GUID; }
        }

        public bool IsOleControl
        {
            get { return (Iid == IID_IOleControl); }
        }

        public bool IsDispatch
        {
            get { return (Iid == IID_IDispatch); }
        }

        public bool IsMarshal
        {
            get { return (Iid == IID_IMarshal); }
        }

        public bool IsPersistStream
        {
            get { return (Iid == IID_IPersistStream) || (Iid == IID_IPersistStreamInit); }
        }

        public bool IsClassFactory
        {
            get { return Iid == typeof(IClassFactory).GUID; }
        }

        public static COMInterfaceEntry CreateKnownInterface(KnownInterfaces known)
        {
            COMInterfaceEntry ent = null;
            switch (known)
            {
                case KnownInterfaces.IUnknown:
                    ent = new COMInterfaceEntry();
                    ent.Base = "";
                    ent.Iid = new Guid("{00000000-0000-0000-C000-000000000046}");
                    ent.ProxyClsid = Guid.Empty;
                    ent.NumMethods = 3;
                    ent.Name = "IUnknown";
                    break;
                case KnownInterfaces.IMarshal:
                    ent = new COMInterfaceEntry();
                    ent.Base = "";
                    ent.Iid = new Guid("{00000003-0000-0000-C000-000000000046}");
                    ent.ProxyClsid = Guid.Empty;
                    ent.NumMethods = 9;
                    ent.Name = "IMarshal";
                    break;
            }

            return ent;
        }

        public string Name
        {
            get; private set;
        }

        public Guid Iid
        {
            get; private set;        
        }

        public Guid ProxyClsid
        {
            get; private set;        
        }

        public int NumMethods
        {
            get; private set;
        }

        public string Base
        {
            get; private set;
        }

        public Guid TypeLib
        {
            get; private set;
        }

        public string TypeLibVersion
        {
            get; private set;
        }

        public override string ToString()
        {
            return String.Format("COMInterfaceEntry: {0}", Name);
        }
    }
}
