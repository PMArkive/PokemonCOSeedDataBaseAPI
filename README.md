# PokemonCOSeedDataBaseAPI 仕様書
# Auther
[@sub_827](https://twitter.com/sub_827)

# 1. 公開Enum
## 1.1. PlayerName
### 1.1.0. 説明
とにかくバトルで表示されるプレイヤー名を表す列挙型.

### 1.1.1. 要素
|要素名|対応するプレイヤー名(日本語)|(英語)|
|:-|:-|:-|
|LEO|レオ|Wes|
|YUTA|ユータ|Seth|
|TATSUKI|タツキ|Thomas|

## 1.2. BattleTeam
### 1.2.0. 説明
とにかくバトルで設定されている構築済みパーティを表す列挙型.

### 1.2.1. 要素
|要素名|左上のポケモン|
|:-|:-|
|Blazikin|バシャーモ|
|Entei|エンテイ|
|Swampert|ラグラージ|
|Raikou|ライコウ|
|Meganium|メガニウム|
|Suicun|スイクン|
|Metagross|メタグロス|
|Heracross|ヘラクロス|

# 2. 公開class
## 2.1. SeedSearcher
### 2.1.0. 説明
DBからseedを検索するためのクラス. 
コンストラクタは非公開なので, インスタンスを得る場合は後述のCreateXXXメソッドを利用すること.
### 2.1.1. 公開フィールド
|シグネチャ|説明|
|:-|:-|
|`public readonly int SpecifiedNumberOfKey`|`Search()`に渡すべきKeyの要素数.|

### 2.1.2. 公開メソッド
|シグネチャ|説明|
|:-|:-|
|`public static SeedSearcher CreateFullDBSearcher(string)`| DBのパスを渡し, FullDBからseedを検索できるインスタンスを得る. |
|`static SeedSearcher CreateLightDBSearcher(string)`| DBのパスを渡し, LightDBからseedを検索できるインスタンスを得る. |
|`public IEnumerable<uint> Search((PlayerName, BattleTeam)[])`| 7回分の連続したとにかくバトル(シングル, 最強)の生成結果から, 現在のseed候補を検索します. 引数の長さが7以外の場合は例外を投げます. |