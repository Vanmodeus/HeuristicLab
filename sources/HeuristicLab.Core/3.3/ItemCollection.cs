#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using HeuristicLab.Collections;
using HeuristicLab.Common.Resources;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Core {
  [EmptyStorableClass]
  [Item("ItemCollection<T>", "Represents a collection of items.")]
  public class ItemCollection<T> : ObservableCollection<T>, IItem where T : class, IItem {
    public virtual string ItemName {
      get { return ItemAttribute.GetName(this.GetType()); }
    }
    public virtual string ItemDescription {
      get { return ItemAttribute.GetDescription(this.GetType()); }
    }
    public virtual Image ItemImage {
      get { return VS2008ImageLibrary.Class; }
    }

    public ItemCollection() : base() { }
    public ItemCollection(int capacity) : base(capacity) { }
    public ItemCollection(IEnumerable<T> collection) : base(collection) { }

    public object Clone() {
      return Clone(new Cloner());
    }

    public virtual IDeepCloneable Clone(Cloner cloner) {
      ItemCollection<T> clone = (ItemCollection<T>)Activator.CreateInstance(this.GetType(), this.Select(x => (T)cloner.Clone(x)));
      cloner.RegisterClonedObject(this, clone);
      return clone;
    }

    public override string ToString() {
      return ItemName;
    }

    public event EventHandler ToStringChanged;
    protected virtual void OnToStringChanged() {
      if (ToStringChanged != null)
        ToStringChanged(this, EventArgs.Empty);
    }
  }
}
