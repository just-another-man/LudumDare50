﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
public class ChangeVisitor : MonoBehaviour
   {
        public static ChangeVisitor instance;

        private SpriteRenderer rend;

            private void Awake()
            {
                if (instance is null)
                    instance = this;
                else   
                {
                    Debug.LogError("double singleton:"+this.GetType().Name);
                }

                rend = GetComponent<SpriteRenderer>();
            }    
            public void Enter(Villager villager)
            {
                rend.sprite = villager.image;
            }

            public void Exit()
            {
                rend.sprite = null;
            }
    }
}