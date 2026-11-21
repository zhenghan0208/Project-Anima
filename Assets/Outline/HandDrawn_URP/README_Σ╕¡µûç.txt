Unity URP 手绘铅笔排线材质
========================
1. 将整个 HandDrawn_URP 文件夹拖进 Unity Assets。
2. Project > Create > Material，命名 HandDrawn_Cream。
3. 在 Material 的 Shader 下拉列表选择 Custom > URP_Stylized_HandDrawn。
4. 拖入纹理：
   Paper Texture (Albedo) = Paper_Albedo.png
   Ramp (Light/Shadow gradient) = Ramp_Cream.png
   Hatching Lines Texture = Hatching_Pencil.png
5. 在 Import Settings 中，Ramp_Cream 的 Wrap Mode 选 Clamp。
6. 建议初始参数：
   Base Color = 奶油白色 (#F4EDDC)
   Hatch Tiling XY = X 6.68, Y 0.48
   Hatching Opacity = 0.8-1.0
   Shadow Threshold = 0.65
   Shadow Strength = 0.9
   Custom Light Blend = 1
   Custom Light Direction = (35.3, 35.37, -3.9)
   Outline Width = 0.0193（按照模型大小调整）
   Outline Color = 深紫黑色 (#231A2B)
7. 将材质拖到 Sphere 或其他带 UV 的 3D 模型上。

说明：
- 这是 URP Shader 源文件，不是 .mat 文件；Unity 导入后自己创建 Material 即可。
- 排线跟随模型 UV，类似截图的纵向铅笔阴影。若某些模型 UV 不佳，排线可能变形。
- 描边是反面膨胀法，宽度随模型尺度及视角变化。
- 若 Unity 特定 URP 版本提示错误，请把 Console 的错误信息发来。
