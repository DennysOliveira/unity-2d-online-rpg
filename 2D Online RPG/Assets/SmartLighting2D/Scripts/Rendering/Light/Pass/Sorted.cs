using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightTilemapCollider;
using LightSettings;

namespace Rendering.Light {
        
    public static class Sorted {

        public static void Draw(Pass pass) {
            for(int i = 0; i < pass.sortPass.sortList.count; i ++) {
                pass.sortPass.sortObject = pass.sortPass.sortList.list[i];

                switch (pass.sortPass.sortObject.type) {
                    case Sorting.SortObject.Type.Collider:
                        DrawCollider(pass);
                    break;

                    case Sorting.SortObject.Type.Tile:
                        DrawTile(pass);
                    break;

                     case Sorting.SortObject.Type.TilemapMap:
                        DrawTileMap(pass);
                    break;

                }
            }
        }

        private static void DrawCollider(Pass pass) {
            LightCollider2D collider = (LightCollider2D)pass.sortPass.sortObject.lightObject;

            if (collider.shadowLayer == pass.layerID && pass.drawShadows) {	

                if (collider.ShadowDisabled() == false) {
                    ShadowEngine.GetMaterial().SetPass(0);

                    GL.Begin(GL.TRIANGLES);

                        Shadow.Shape.Draw(pass.light, collider);
        
                    GL.End();
                }

            }

            // Masking
            if (collider.maskLayer == pass.layerID && pass.drawMask) {
                pass.materialWhite.color = Color.white;

                switch(collider.mainShape.maskType) {
                    case LightCollider2D.MaskType.SpritePhysicsShape:
                    case LightCollider2D.MaskType.Collider2D:
                    case LightCollider2D.MaskType.Collider3D:
                    case LightCollider2D.MaskType.CompositeCollider2D:
                    
                        pass.materialWhite.SetPass(0);
                        GL.Begin(GL.TRIANGLES);
                            Shape.Mask(pass.light, collider, pass.layer);
                        GL.End();

                    break;

                    case LightCollider2D.MaskType.Sprite:

                        SpriteRenderer2D.Mask(pass.light, collider, pass.materialWhite, pass.layer);
                        
                        break;

                    case LightCollider2D.MaskType.BumpedSprite:
                
                        Material material = collider.bumpMapMode.SelectMaterial(pass.materialNormalMap_PixelToLight, pass.materialNormalMap_ObjectToLight);
                        SpriteRenderer2D.MaskNormalMap(pass.light, collider, material, pass.layer);
                        
                        break;

                    case LightCollider2D.MaskType.MeshRenderer:
                    
                        pass.materialWhite.SetPass(0);
                        GL.Begin(GL.TRIANGLES);
                            Mesh.Mask(pass.light, collider, pass.materialWhite, pass.layer);
                        GL.End();

                    break;

                    case LightCollider2D.MaskType.BumpedMeshRenderer:

                        Material material2 = collider.bumpMapMode.SelectMaterial(pass.materialNormalMap_PixelToLight, pass.materialNormalMap_ObjectToLight);
                        material2.SetPass(0);
                        GL.Begin(GL.TRIANGLES);
                        Mesh.MaskNormalMap(pass.light, collider, material2, pass.layer);
                        GL.End();

                        break;

                    case LightCollider2D.MaskType.SkinnedMeshRenderer:

                        pass.materialWhite.SetPass(0);
                        GL.Begin(GL.TRIANGLES);
                            SkinnedMesh.Mask(pass.light, collider, pass.materialWhite, pass.layer);
                        GL.End();

                    break;
                }
            }
        }

        private static void DrawTile(Pass pass) {
            #if UNITY_2017_4_OR_NEWER

                LightingTile tile = (LightingTile)pass.sortPass.sortObject.lightObject;
                LightTilemapCollider2D tilemap = pass.sortPass.sortObject.tilemap;
                
                bool masksDisabled = tilemap.MasksDisabled();
                bool shadowsDisabled = tilemap.ShadowsDisabled();
                
                if (pass.sortPass.sortObject.tilemap.shadowLayer == pass.layerID && pass.drawShadows && shadowsDisabled == false) {

                    ShadowEngine.GetMaterial().SetPass(0);

                    GL.Begin(GL.TRIANGLES);

                        Shadow.Tile.Draw(pass.light, tile, tilemap);
        
                    GL.End();
                    
                }

                // sprite mask - but what about shape mask?
                if (tilemap.maskLayer == pass.layerID && pass.drawMask && masksDisabled == false) {
                    Tile.MaskSprite(tile, pass.layer, pass.materialWhite, tilemap, pass.lightSizeSquared);
                }    

             #endif     
        }

        private static void DrawTileMap(Pass pass) {
            #if UNITY_2017_4_OR_NEWER

                LightTilemapCollider2D tilemap = pass.sortPass.sortObject.tilemap;

                bool masksDisabled = tilemap.MasksDisabled();
                bool shadowsDisabled = tilemap.ShadowsDisabled();
               
                if (tilemap.shadowLayer == pass.layerID && pass.drawShadows && shadowsDisabled == false) {	

                    ShadowEngine.GetMaterial().SetPass(0);
                                        
                    GL.Begin(GL.TRIANGLES);

                     switch(tilemap.mapType) {
                        case MapType.UnityRectangle:

                            switch(tilemap.rectangle.shadowType) {
                                case ShadowType.Grid:
                                case ShadowType.SpritePhysicsShape:
                                    Shadow.UnityTilemap.Draw(pass.light, tilemap, pass.lightSizeSquared);
                                break;

                                case ShadowType.CompositeCollider:
                                    Shadow.TilemapCollider.Rectangle.Draw(pass.light, tilemap);
                                break;
                            }
                            
                        break;

                        case MapType.UnityIsometric:

                            switch(tilemap.isometric.shadowType) {
                                case ShadowType.Grid:
                                case ShadowType.SpritePhysicsShape:
                                     Shadow.UnityTilemap.Draw(pass.light, tilemap, pass.lightSizeSquared);
                                break;
                            }
                           
                        break;

                        case MapType.UnityHexagon:

                            switch(tilemap.hexagon.shadowType) {
                                case ShadowType.Grid:
                                case ShadowType.SpritePhysicsShape:
                                     Shadow.UnityTilemap.Draw(pass.light, tilemap, pass.lightSizeSquared);
                                break;
                            }
                            
                        break;

                        case MapType.SuperTilemapEditor:

                            switch(tilemap.superTilemapEditor.shadowTypeSTE) {

                                case SuperTilemapEditorSupport.TilemapCollider2D.ShadowType.TileCollider:
                                case SuperTilemapEditorSupport.TilemapCollider2D.ShadowType.Grid:
                                        SuperTilemapEditorSupport.RenderingColliderShadow.Grid(pass.light, tilemap);
                                    break;
                                    
                                case SuperTilemapEditorSupport.TilemapCollider2D.ShadowType.Collider:
                                        SuperTilemapEditorSupport.RenderingColliderShadow.Collider(pass.light, tilemap);
                                    break;
                                }
                            
                        break;
                    }

                    GL.End();  
                }

                if (pass.sortPass.sortObject.tilemap.maskLayer == pass.layerID && pass.drawMask && masksDisabled == false) {
                    switch(tilemap.mapType) {
                        case MapType.UnityRectangle:

                            switch(tilemap.rectangle.maskType) {
                                case LightTilemapCollider.MaskType.Sprite:
                                    UnityTilemap.Sprite(pass.light, tilemap, pass.materialWhite, pass.layer);
                                break;
                                
                                case LightTilemapCollider.MaskType.BumpedSprite:

                                    Material material = tilemap.bumpMapMode.SelectMaterial(pass.materialNormalMap_PixelToLight, pass.materialNormalMap_ObjectToLight);
                                    UnityTilemap.BumpedSprite(pass.light, tilemap, material, pass.layer);
                            
                                break;
                            }
                            
                        break;

                        case MapType.UnityIsometric:

                            switch(tilemap.isometric.maskType) {
                                case MaskType.Sprite:
                                    UnityTilemap.Sprite(pass.light, tilemap, pass.materialWhite, pass.layer);
                                break;
                            }
                            
                        break;

                        case MapType.UnityHexagon:

                            switch(tilemap.hexagon.maskType) {
                                case MaskType.Sprite:
                                //    TilemapHexagon.MaskSprite(pass.buffer, tilemap, pass.materialWhite, pass.z);
                                break;
                            }
                            
                        break;

                        case MapType.SuperTilemapEditor:

                            switch(tilemap.superTilemapEditor.maskTypeSTE) {
                                case SuperTilemapEditorSupport.TilemapCollider.MaskType.Sprite:

                                    SuperTilemapEditorSupport.RenderingColliderMask.WithoutAtlas.Sprite(pass.light, tilemap, pass.materialWhite);
                                
                                break;
                                
                                case SuperTilemapEditorSupport.TilemapCollider.MaskType.BumpedSprite:

                                    Material material = tilemap.bumpMapMode.SelectMaterial(pass.materialNormalMap_PixelToLight, pass.materialNormalMap_ObjectToLight);
                                    SuperTilemapEditorSupport.RenderingColliderMask.WithoutAtlas.BumpedSprite(pass.light, tilemap, material);
                            
                                break;
                            }

                            
                            
                        break;
                    }   
                }    

             #endif     
        }
    }
}