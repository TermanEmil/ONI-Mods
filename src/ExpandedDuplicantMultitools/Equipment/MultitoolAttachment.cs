using System.Collections.Generic;
using Klei.AI;
using UnityEngine;
using UnityEngine.Serialization;

namespace ExpandedDuplicantMultitools.Equipment
{
    public class MultitoolAttachment : KMonoBehaviour
    { 
        public AttachmentType toolType;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Debug.Log("Attachment added");
        }

        public enum AttachmentType
        {
            None,
            NeutroniumMiner,
        }
    }
}