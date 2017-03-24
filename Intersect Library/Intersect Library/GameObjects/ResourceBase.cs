﻿using System;
using System.Collections.Generic;
using Intersect.GameObjects.Conditions;
using Intersect.Localization;

namespace Intersect.GameObjects
{
    public class ResourceBase : DatabaseObject<ResourceBase>
    {
        // General
        public new const string DATABASE_TABLE = "resources";
        public new const GameObject OBJECT_TYPE = GameObject.Resource;
        protected static Dictionary<int, DatabaseObject> Objects = new Dictionary<int, DatabaseObject>();
        public int Animation = 0;

        // Drops
        public List<ResourceDrop> Drops = new List<ResourceDrop>();
        public string EndGraphic = Strings.Get("general", "none");

        public ConditionLists HarvestingReqs = new ConditionLists();

        // Graphics
        public string InitialGraphic = Strings.Get("general", "none");
        public int MaxHP = 0;

        public int MinHP = 0;
        public int SpawnDuration = 0;
        public int Tool = -1;
        public bool WalkableAfter = false;
        public bool WalkableBefore = false;

        public ResourceBase(int id) : base(id)
        {
            Name = "New Resource";
            for (int i = 0; i < Options.MaxNpcDrops; i++)
            {
                Drops.Add(new ResourceDrop());
            }
        }

        public override void Load(byte[] packet)
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(packet);
            Name = myBuffer.ReadString();
            InitialGraphic = myBuffer.ReadString();
            EndGraphic = myBuffer.ReadString();
            MinHP = myBuffer.ReadInteger();
            MaxHP = myBuffer.ReadInteger();
            Tool = myBuffer.ReadInteger();
            SpawnDuration = myBuffer.ReadInteger();
            Animation = myBuffer.ReadInteger();
            WalkableBefore = Convert.ToBoolean(myBuffer.ReadInteger());
            WalkableAfter = Convert.ToBoolean(myBuffer.ReadInteger());

            for (int i = 0; i < Options.MaxNpcDrops; i++)
            {
                Drops[i].ItemNum = myBuffer.ReadInteger();
                Drops[i].Amount = myBuffer.ReadInteger();
                Drops[i].Chance = myBuffer.ReadInteger();
            }

            HarvestingReqs.Load(myBuffer);

            myBuffer.Dispose();
        }

        public byte[] ResourceData()
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteString(Name);
            myBuffer.WriteString(InitialGraphic);
            myBuffer.WriteString(EndGraphic);
            myBuffer.WriteInteger(MinHP);
            myBuffer.WriteInteger(MaxHP);
            myBuffer.WriteInteger(Tool);
            myBuffer.WriteInteger(SpawnDuration);
            myBuffer.WriteInteger(Animation);
            myBuffer.WriteInteger(Convert.ToInt32(WalkableBefore));
            myBuffer.WriteInteger(Convert.ToInt32(WalkableAfter));

            for (int i = 0; i < Options.MaxNpcDrops; i++)
            {
                myBuffer.WriteInteger(Drops[i].ItemNum);
                myBuffer.WriteInteger(Drops[i].Amount);
                myBuffer.WriteInteger(Drops[i].Chance);
            }

            HarvestingReqs.Save(myBuffer);

            return myBuffer.ToArray();
        }

        public override byte[] BinaryData => ResourceData();
        public override string DatabaseTableName => DATABASE_TABLE;
        public override GameObject GameObjectType => OBJECT_TYPE;

        public class ResourceDrop
        {
            public int Amount;
            public int Chance;
            public int ItemNum;
        }
    }
}