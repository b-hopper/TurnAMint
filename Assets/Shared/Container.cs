using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Container : MonoBehaviour {

    [Serializable]
    public class ContainerItem
    {
        public Guid Id;
        public string Name;
        public int MaximumCapacity;

        public int amountTaken;

        public ContainerItem()
        {
            Id = Guid.NewGuid();
        }

        public int Remaining
        {
            get
            {
                return MaximumCapacity - amountTaken;
            }
        }

        internal int Get(int value)
        {
            if (amountTaken + value > MaximumCapacity)
            {
                int tooMuch = (amountTaken + value) - MaximumCapacity;
                amountTaken = MaximumCapacity;
                return value - tooMuch;
            }
            amountTaken += value;
            return value;
        }

        internal void Set(int amount)
        {
            amountTaken -= amount;
            if (amountTaken < 0)
            {
                amountTaken = 0;
            }
        }
    }
    
    public List<ContainerItem> items;
    public event Action OnContainerReady;

    private void Awake()
    {
        items = new List<ContainerItem>();
        if (OnContainerReady != null)
        {
            OnContainerReady();
        }
    }

    public Guid Add(string name, int maximumCapacity)
    {
        items.Add(new ContainerItem
        {
            MaximumCapacity = maximumCapacity,
            Name = name
        });
        
        return items.Last().Id;
    }

    public void Put(string name, int amount)
    {
        var containerItem = items.Where(x => x.Name == name).FirstOrDefault();
        if (containerItem == null)
        {
            return;
        }
        containerItem.Set(amount);
    }

    public int RemoveFromContainer(Guid id, int amount)
    {
        var containerItem = GetContainerItem(id);
        if (containerItem == null)
        {
            return -1;
        }
        return containerItem.Get(amount);
    }

    public int GetAmountRemaining(Guid id)
    {
        var containerItem = GetContainerItem(id);
        if (containerItem == null)
        {
            return -1; 
        }
        return containerItem.Remaining;
    }

    ContainerItem GetContainerItem(Guid id)
    {
        var containerItem = items.Where(x => x.Id == id).FirstOrDefault();
        return containerItem;
    }
}
