﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace HeuristicLab.Hive.Server.Core {
  public class PolicyCollection : IEnumerable<Policy> {
    private IList<Policy> _policies = new List<Policy>();

    public IList<Policy> Policies { get { return this._policies; } }

    public Policy this[string name] {
      get {
        foreach (Policy item in _policies) {
          if (item.Name == name)
            return item;
        }
        return null;
      }
    }

    public IEnumerator<Policy> GetEnumerator() {
      return this._policies.GetEnumerator();
    }

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this._policies.GetEnumerator();
    }

    #endregion
  }
}
