using System.Diagnostics.CodeAnalysis;

namespace Labyrinth.Items
{
    /// <summary>
    /// Inventory of collectable items for rooms and players.
    /// </summary>
    /// <param name="item">Optional initial item in the inventory.</param>
    public abstract class Inventory(ICollectable? item = null)
    {
        protected List<ICollectable>? _items = item is not null ? [item] : [];

        /// <summary>
        /// True if the room has an item, false otherwise.
        /// </summary>
        [MemberNotNullWhen(true, nameof(_items))]
        public bool HasItems => _items is { Count: > 0 };
        
        /// <summary>
        /// Number of items in the inventory.
        /// </summary>
        public int ItemCount => _items?.Count ?? 0;
        

        /// <summary>
        /// Gets the type of the item in the room.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the room has no item (check with <see cref="HasItems"/>).</exception>
        public IEnumerable<Type> ItemTypes {
            get {
                return _items != null ? _items.Select(item => item.GetType()) : throw new InvalidOperationException("No item in the room");
            }
        }
        
        /// <summary>
        /// Places an item in the inventory, removing it from another one.
        /// </summary>
        /// <param name="from">The inventory from which the item is taken. The item is removed from this inventory.</param>
        /// <exception cref="InvalidOperationException">Thrown if the room already contains an item (check with <see cref="HasItems"/>).</exception>
        [MemberNotNull(nameof(_items))]
        public void MoveItemFrom(Inventory from, int nth = 0) {
            if (!from.HasItems) throw new InvalidOperationException("Source inventory has no item");

            var item = from._items[nth];
            from._items.RemoveAt(nth);
            _items.Add(item);
        }
    }
}
