using System;
using Flecs.NET.Utilities;
using static Flecs.NET.Bindings.Native;

namespace Flecs.NET.Core
{
    public static unsafe class Invoker
    {
        public static void Iter(Ecs.IterCallback iterCallback, Ecs.IterNext iterNext, ecs_iter_t* iter)
        {
            if (iterCallback == null)
                throw new ArgumentNullException(nameof(iterCallback));

            if (iterNext == null)
                throw new ArgumentNullException(nameof(iterNext));

            while (iterNext(iter) == 1)
            {
                Macros.TableLock(iter->world, iter->table);
                iterCallback(new Iter(iter));
                Macros.TableUnlock(iter->world, iter->table);
            }
        }

        public static void Each(Ecs.EachCallback eachCallback, Ecs.IterNext iterNext, ecs_iter_t* iter)
        {
            if (eachCallback == null)
                throw new ArgumentNullException(nameof(eachCallback));

            if (iterNext == null)
                throw new ArgumentNullException(nameof(iterNext));

            while (iterNext(iter) == 1)
            {
                Macros.TableLock(iter->world, iter->table);

                ecs_world_t* world = iter->world;
                int count = iter->count;

                Assert.True(count > 0, "No entities returned, use Each() without Entity argument");

                for (int i = 0; i < count; i++)
                    eachCallback(new Entity(world, iter->entities[i]));

                Macros.TableUnlock(iter->world, iter->table);
            }
        }
    }
}