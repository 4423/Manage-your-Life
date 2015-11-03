using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Livet;
using System.Runtime.CompilerServices;

namespace Manage_your_Life.ViewModels
{
    public class BindableBase : Livet.ViewModel
    {
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value)) return false;

            storage = value;
            this.RaisePropertyChanged(propertyName);
            return true;
        }
    }
}
