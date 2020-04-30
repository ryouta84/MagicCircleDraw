1. 32x32 or 64x64 の図形画像を用意する。図形は黒で背景は透明にする。
2. Draw フォルダにimport する。
3. Alpah is transparency と Read/Write　Enabled をオンにする。
4. マテリアルを作成しAlbedoに指定する。ShaderをTransparentがはいってるものにする。
5. MagicCircleCanvasプレハブをインスタンス化したオブジェクトをDrawHandのDrawコンポーネントにセットする。