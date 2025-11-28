using NUnit.Framework;
using Labyrinth.Build;
using Labyrinth.Tiles;
using Labyrinth.Items;
using System;

namespace Labyrinth.Tests
{
    [TestFixture]
    public class KeyMasterTest
    {
        private static Room[] CreateRooms(Keymaster keymaster, int numberOfRooms) {
            var rooms = new Room[numberOfRooms];
            for (var i = 0; i < numberOfRooms; i++) {
                rooms[i] = keymaster.NewKeyRoom();
            }
            return rooms;
        }
        
        private static Door[] CreateDoors(Keymaster keymaster, int numberOfDoors) {
            var doors = new Door[numberOfDoors];
            for (var i = 0; i < numberOfDoors; i++) {
                doors[i] = keymaster.NewDoor();
            }
            return doors;
        }
        
        private static bool[] PassAllRooms(Room[] rooms, Door[] doors) {
            var results = new bool[rooms.Length];
            for (var i = 0; i < rooms.Length; i++) {
                var keyInventory = rooms[i].Pass();
                results[i] = doors[i].Open(keyInventory);
            }
            return results;
        }
        private static void AssertThatResultDoorUnlocked(bool[] results)
        {
            for (var i = 0; i < 3; i++) {
                Assert.That(results[i], Is.True, $"Door {i} should open with its corresponding key.");
            }
        }
        
        [Test]
        public void SingleDoorThenRoom_AssignsKeyCorrectly() {
            using var keymaster = new Keymaster();

            var door = keymaster.NewDoor();
            var room = keymaster.NewKeyRoom();
            var keyInventory = room.Pass();
            bool opened = door.Open(keyInventory);

            Assert.That(door.IsLocked, Is.False);
            Assert.That(opened, Is.True);
        }

        [Test]
        public void MultipleDoorsAndRooms_AssignKeysInOrder() {
            using var keymaster = new Keymaster();
            var doors = CreateDoors(keymaster, 3);
            var rooms = CreateRooms(keymaster, 3);
            var results = PassAllRooms(rooms, doors);
            
            using var all = Assert.EnterMultipleScope();
            AssertThatResultDoorUnlocked(results);
        }

        [Test]
        public void MultipleRoomsThenDoors_AssignKeysInOrder() {
            using var keymaster = new Keymaster();
            var rooms = CreateRooms(keymaster, 3);
            var doors = CreateDoors(keymaster, 3);
            var results = PassAllRooms(rooms, doors);
            
            using var all = Assert.EnterMultipleScope();
            AssertThatResultDoorUnlocked(results);
        }

        [Test]
        public void Dispose_WithUnmatchedKeysOrRooms_Throws() {
            var keymaster = new Keymaster();
            keymaster.NewDoor();

            Assert.Throws<InvalidOperationException>(() => keymaster.Dispose(),
                "Dispose should throw if unmatched keys or rooms exist.");
        }

        [Test]
        public void DoorsAndRooms_MixedOrder_AssignsAllKeysCorrectly() {
            using var keymaster = new Keymaster();
            var doors = new Door[3];
            var rooms = new Room[3];

            rooms[0] = keymaster.NewKeyRoom();
            doors[0] = keymaster.NewDoor();
            doors[1] = keymaster.NewDoor();
            rooms[1] = keymaster.NewKeyRoom();
            rooms[2] = keymaster.NewKeyRoom();
            doors[2] = keymaster.NewDoor();
            
            var results = PassAllRooms(rooms, doors);
            
            using var all = Assert.EnterMultipleScope();
            AssertThatResultDoorUnlocked(results);
        }
    }
}
