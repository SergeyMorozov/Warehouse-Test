using System;
using Unity.Mathematics;
using UnityEngine;

namespace  GAME
{
    public class PlayerLogic : MonoBehaviour
    {
        private PlayerObject _player;
        private int _layerMask;
        
        private void Awake()
        {
            _player = PlayerSystem.Data.CurrentPlayer;
            _layerMask = LayerMask.GetMask("Item");
        }

        private void Update()
        {
            if (_player.ItemInHand != null && Input.GetMouseButtonDown(0))
            {
                _player.ItemInHand.Ref.Rigidbody.isKinematic = false;
                _player.ItemInHand.Ref.MeshCollider.enabled = true;
                _player.ItemInHand.transform.SetParent(null);

                _player.ItemInHand.Ref.Rigidbody.AddForce(
                    (_player.Ref.Camera.transform.forward + Vector3.up * 0.3f) * _player.Preset.DropForce,
                    ForceMode.Impulse);
                _player.ItemInHand = null;
                return;
            }

            Ray ray = _player.Ref.Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit, 6, _layerMask))
            {
                return;
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                ItemObject item = hit.collider.GetComponentInParent<ItemObject>();
                if(item == null) return;
                
                item.Ref.Rigidbody.isKinematic = true;
                item.Ref.MeshCollider.enabled = false;

                _player.ItemInHand = item;
                item.transform.SetParent(_player.Ref.Hand);
                item.transform.localPosition = Vector3.zero;
                item.transform.localRotation = quaternion.identity;
            }
        }
    }
}

