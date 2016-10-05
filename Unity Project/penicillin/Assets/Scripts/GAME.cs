﻿using UnityEngine;
using System.Collections;

namespace GLOBAL{
    
    public class GAME : MonoBehaviour {


        public const int jumps = 1;
        public const int dashes = 3;
        public const int max_health = 10;
        public const int tile_size = 64;

        public const float invulnerable_timer = 2f;
        public const float player_velocity = 3f;
		public const float dash_force = 250f;
        public const float dash_timer = 0.5f;
        public const float dash_cooldown = 2f;
        public const float jump_force = 250f;
        public const float jump_anim_loop = 1.05f;
        public const float acid_dot_timer = 2f;
        public const float jump_velocity = 5;
    }

}


