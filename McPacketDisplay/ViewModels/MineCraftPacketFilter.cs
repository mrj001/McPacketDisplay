using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using McPacketDisplay.Models.Packets;

namespace McPacketDisplay.ViewModels
{
   public class MineCraftPacketFilter : ICollection<MineCraftPacketFilterElement>,
            INotifyCollectionChanged, INotifyPropertyChanged
   {
      private readonly List<MineCraftPacketFilterElement> _filters;

      private int _serial = 0;

      private bool _suppressSerial = false;

      #region Construction
      public MineCraftPacketFilter(IMineCraftProtocol protocol)
      {
         _filters = new List<MineCraftPacketFilterElement>(protocol.Count);

         foreach(IMineCraftPacketDefinition definition in protocol)
         {
            MineCraftPacketFilterElement element = new MineCraftPacketFilterElement(definition);
            element.PropertyChanged += HandleElementChanged;
            _filters.Add(element);
         }
      }
      #endregion

      private void HandleElementChanged(object? sender, PropertyChangedEventArgs e)
      {
         if (e.PropertyName != nameof(MineCraftPacketFilterElement.Pass))
            return;
         this.Serial++;
         OnItemChanged((MineCraftPacketFilterElement)sender!);
      }

      /// <summary>
      /// Updates the PacketCount properties of each MineCraftPacketFilterElement.
      /// </summary>
      /// <param name="packets"></param>
      public void UpdateFilterPacketCounts(IEnumerable<IMineCraftPacket> packets)
      {
         // NOTE: Potential performance impact.  Many PropertyChanged events will be raised
         //   by this method for PacketCount.

         try
         {
            _suppressSerial = true;

            // Reset Packet Counts to zero
            foreach (MineCraftPacketFilterElement element in _filters)
               element.PacketCount = 0;

            // Count Packets
            foreach(IMineCraftPacket packet in packets)
            {
               MineCraftPacketFilterElement? element = GetFilterElement(packet);

               if (element is null)
               {
                  element = new MineCraftPacketFilterElement(packet);
                  _filters.Add(element);
               }

               element.PacketCount += 1;
            }
         }
         finally
         {
            _suppressSerial = false;
         }

         OnCollectionReset();
      }

      /// <summary>
      /// Gets whether or not the given MineCraft Packet passes this Filter.
      /// </summary>
      /// <param name="packet">The MineCraft Packet to check.</param>
      /// <returns>True if the Packet passes the Filter; false if not.</returns>
      public virtual bool Pass(IMineCraftPacket packet)
      {
         MineCraftPacketFilterElement? element = GetFilterElement(packet);
         bool rv =  element?.Pass ?? true;
         return rv;
      }

      /// <summary>
      /// Gets the MineCraft Packet Filter Element corresponding to the given MineCraft Packet.
      /// </summary>
      /// <param name="packet"></param>
      /// <returns></returns>
      private MineCraftPacketFilterElement? GetFilterElement(IMineCraftPacket packet)
      {
         // NOTE: Linear search => potential performance impact.
         return _filters.Where(a => a.IsMatch(packet)).FirstOrDefault();
      }

      /// <summary>
      /// This number is updated when members change.  It's used to cause
      /// re-application of this filter when one of its elements is changed.
      /// </summary>
      public int Serial
      {
         get
         {
            return _serial;
         }
         protected set
         {
            if (_suppressSerial || value == _serial)
               return;
            _serial = value;
            OnPropertyChanged();
         }
      }

      #region ICollection<MineCraftPacketFilterElement>
      public void Add(MineCraftPacketFilterElement item)
      {
         _filters.Add(item);
         OnItemAdded(item);
      }

      public void Clear()
      {
         _filters.Clear();
         OnCollectionReset();
      }

      public bool Contains(MineCraftPacketFilterElement item)
      {
         return _filters.Contains(item);
      }

      public void CopyTo(MineCraftPacketFilterElement[] array, int arrayIndex)
      {
         CopyTo(array, arrayIndex);
      }

      public bool Remove(MineCraftPacketFilterElement item)
      {
         bool rv = _filters.Remove(item);
         if (rv)
            OnItemRemoved(item);
         return rv;
      }

      public int Count => _filters.Count;

      public bool IsReadOnly => false;

      public IEnumerator<MineCraftPacketFilterElement> GetEnumerator()
      {
         return _filters.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return _filters.GetEnumerator();
      }
      #endregion

      #region INotifyPropertyChanged
      public event PropertyChangedEventHandler? PropertyChanged;

      protected virtual void OnPropertyChanged([CallerMemberName]string? property = null)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
      }
      #endregion

      #region INotifyCollectionChanged
      public event NotifyCollectionChangedEventHandler? CollectionChanged;

      protected virtual void OnCollectionReset()
      {
         CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
      }

      protected virtual void OnItemAdded(MineCraftPacketFilterElement newElement)
      {
         CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newElement));
      }

      protected virtual void OnItemRemoved(MineCraftPacketFilterElement removed)
      {
         CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed));
      }

      protected virtual void OnItemChanged(MineCraftPacketFilterElement changedElement)
      {
         CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, changedElement, changedElement));
      }
      #endregion

      /*
            #region IList<MineCraftPacketFilterElement>
            public MineCraftPacketFilterElement this[int index]
            {
               get => _filters[index];
               set => _filters[index] = value;
            }

            public int Count => _filters.Count;

            public bool IsReadOnly => false;

            public void Add(MineCraftPacketFilterElement item)
            {
               _filters.Add(item);
            }

            public void Clear()
            {
               _filters.Clear();
            }

            public bool Contains(MineCraftPacketFilterElement item)
            {
               return _filters.Contains(item);
            }

            public void CopyTo(MineCraftPacketFilterElement[] array, int arrayIndex)
            {
               CopyTo(array, arrayIndex);
            }

            public int IndexOf(MineCraftPacketFilterElement item)
            {
               return _filters.IndexOf(item);
            }

            public void Insert(int index, MineCraftPacketFilterElement item)
            {
               _filters.Insert(index, item);
            }

            public bool Remove(MineCraftPacketFilterElement item)
            {
               return _filters.Remove(item);
            }

            public void RemoveAt(int index)
            {
               _filters.RemoveAt(index);
            }

            public IEnumerator<MineCraftPacketFilterElement> GetEnumerator()
            {
               return _filters.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
               return _filters.GetEnumerator();
            }
            #endregion
      */
   }
}
