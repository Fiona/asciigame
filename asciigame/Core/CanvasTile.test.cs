using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NUnit.Framework;


namespace asciigame.Core
{
    [TestFixture]
    public class CanvasTileLayerTests
    {

        [Test]
        public void TileLayerEquality()
        {
            var tile1 = new CanvasTileLayer { character = 'A', colour = Color.White, rotation = 0f };
            var tile2 = new CanvasTileLayer { character = 'A', colour = Color.White, rotation = 0f };
            Assert.That(tile1, Is.EqualTo(tile2));
            Assert.That(tile2, Is.EqualTo(tile1));
        }

        [Test]
        public void TileLayerInequality()
        {
            var tile1 = new CanvasTileLayer { character = 'A', colour = Color.White, rotation = 0f };
            var tile2 = new CanvasTileLayer { character = 'B', colour = Color.White, rotation = 0f };
            Assert.That(tile1, Is.Not.EqualTo(tile2));
            Assert.That(tile2, Is.Not.EqualTo(tile1));
            tile2.colour = Color.Aqua;
            tile2.character = 'A';
            Assert.That(tile1, Is.Not.EqualTo(tile2));
            Assert.That(tile2, Is.Not.EqualTo(tile1));
            tile2.colour = Color.White;
            tile2.rotation = 90f;
            Assert.That(tile1, Is.Not.EqualTo(tile2));
            Assert.That(tile2, Is.Not.EqualTo(tile1));
        }

    }

    [TestFixture]
    public class CanvasTileTests
    {

        [Test]
        public void TileEquality()
        {
            var tile1 = new CanvasTile
            {
                background = Color.White,
                layers = new List<CanvasTileLayer>{
                    new CanvasTileLayer { character = 'C', colour = Color.Olive, rotation = 0f },
                    new CanvasTileLayer { character = 'B', colour = Color.Olive, rotation = 0f },
                    new CanvasTileLayer { character = 'A', colour = Color.Olive, rotation = 0f }
                }
            };
            var tile2 = new CanvasTile
            {
                background = Color.White,
                layers = new List<CanvasTileLayer>{
                    new CanvasTileLayer { character = 'C', colour = Color.Olive, rotation = 0f },
                    new CanvasTileLayer { character = 'B', colour = Color.Olive, rotation = 0f },
                    new CanvasTileLayer { character = 'A', colour = Color.Olive, rotation = 0f }
                }
            };
            Assert.That(tile1.background, Is.EqualTo(tile2.background));
            Assert.That(tile1.layers, Is.EqualTo(tile2.layers));

            Assert.That(tile1, Is.EqualTo(tile2));
            Assert.That(tile2, Is.EqualTo(tile1));
        }

        [Test]
        public void TileInequality()
        {
            var tile1 = new CanvasTile
            {
                background = Color.White,
                layers = new List<CanvasTileLayer>{
                    new CanvasTileLayer { character = 'C', colour = Color.Olive, rotation = 0f }
                }
            };
            var tile2 = new CanvasTile
            {
                background = Color.Black,
                layers = new List<CanvasTileLayer>{
                    new CanvasTileLayer { character = 'C', colour = Color.Olive, rotation = 0f }
                }
            };
            Assert.That(tile1, Is.Not.EqualTo(tile2));
            Assert.That(tile2, Is.Not.EqualTo(tile1));
            tile2.background = Color.White;
            tile2.layers.Add(new CanvasTileLayer {character = 'D', colour = Color.IndianRed, rotation = 0f});
            Assert.That(tile1, Is.Not.EqualTo(tile2));
            Assert.That(tile2, Is.Not.EqualTo(tile1));
        }

    }
}