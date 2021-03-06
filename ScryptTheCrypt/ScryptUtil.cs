﻿using System.Collections.Generic;
using System.Text;

namespace ScryptTheCrypt
{
    public static class ScryptUtil
    {
        public static T[] IListToArray<T>(IList<T> list)
        {
            var retval = new T[list.Count];
            list.CopyTo(retval, 0);
            return retval;
        }
        public class JSFunction
        {
            public readonly string name;
            public readonly string body;
            public JSFunction(string name, string bodyTemplate)
            {
                this.name = name;
                body = string.Format(bodyTemplate, name);
            }
        }
        // this is not the kind of javascript you'd write normally - but because it ECMAScrypt 5.x,
        // and needs to interface with C#, we can't take the usual shortcuts like the
        // return ... && ....; trick
        public static JSFunction chooseRandom = new JSFunction(
            "chooseRandom",
            @"function {0}(actors, rng) {{
                var living = [];
                var e = actors.GetEnumerator();
                while (e.MoveNext()) {{
                    if (e.Current.Alive) {{
                        living.push(e.Current);
                    }}
                }}
                if (!living.length) return null;
                return living[ rng.Next(0, living.length - 1) ];
            }}"
        );
        public static JSFunction attackTargets = new JSFunction(
            "attackTargets",
            @"function {0}(game, actor) {{
                var targets = game.GetTargets();
                for (var i = 0; i < targets.Count; ++i) {{
                    actor.Attack(targets[i]);
                    targets[i].Targeted = false;
                }}
            }}"
        );
        public static readonly string defaultAttack;
        static ScryptUtil()
        {
            var script = new StringBuilder();
            script.AppendLine(ScryptUtil.chooseRandom.body);
            script.AppendLine(ScryptUtil.attackTargets.body);
            script.AppendLine(@"
            function actorActions(g, a) {
                var enemies = a.align == Alignment_Mob ? g.Players : g.Mobs;
                var choice = chooseRandom(enemies, g.rng);
                if (choice) {
                    choice.Targeted = true;
                }
                attackTargets(g, a);
            }
            ");
            defaultAttack = script.ToString();
        }
    }
}
