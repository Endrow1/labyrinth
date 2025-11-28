using Labyrinth.Items;
using Labyrinth.Tiles;

namespace Labyrinth.Build
{
    /// <summary>
    /// Manage the creation of doors and key rooms ensuring each door has a corresponding key room.
    /// </summary>
    public sealed class Keymaster : IDisposable
    {
        private readonly Queue<Inventory> _unplacedKeys = new();
        private readonly Queue<Room> _pendingKeyRooms = new();

        /// <summary>
        /// Ensure all created doors have a corresponding key room and vice versa.
        /// </summary>
        /// <exception cref="InvalidOperationException">Some keys are missing or are not placed.</exception>
        public void Dispose()
        {
            if (_unplacedKeys.Count > 0 || _pendingKeyRooms.Count > 0) {
                throw new InvalidOperationException("Unmatched key/door creation");
            }
        }

        /// <summary>
        /// Create a new door and place its key in a previously created empty key room (if any).
        /// </summary>
        /// <returns>Created door</returns>
        public Door NewDoor()
        {
            var door = new Door();
            
            var keyInventory = new MyInventory();
            door.LockAndTakeKey(keyInventory);
            _unplacedKeys.Enqueue(keyInventory);
            PlaceKey();
            return door;
        }

        /// <summary>
        /// Create a new room with key and place the key if a door was previously created.
        /// </summary>
        /// <returns>Created key room</returns>
        public Room NewKeyRoom()
        {
            var room = new Room();
            _pendingKeyRooms.Enqueue(room);

            PlaceKey();
            return room;
        }

        private void PlaceKey()
        {
            if (_unplacedKeys.Count <= 0 || _pendingKeyRooms.Count <= 0) return;
            var emptyKeyRoom = _pendingKeyRooms.Dequeue();
            var keyInventory = _unplacedKeys.Dequeue();
            emptyKeyRoom.Pass().MoveItemFrom(keyInventory);
        }
    }
}