# StableRef

[English](README.md) | **Русский**

---

Переименовывай классы свободно — сериализованные ссылки не сломаются.

Сериализуемая обёртка для полиморфных ссылок в Unity, которая не ломается при переименовании классов. Построена поверх `[SerializeReference]`: рядом с объектом хранится стабильный строковый ID типа, поэтому переименование или перемещение класса не разрушает уже сохранённые данные.

## Установка

**Через Package Manager (Git URL)**

Открой `Window → Package Manager`, нажми `+` → `Add package from git URL` и введи:

```
https://github.com/EgorShesterikov/Unity-StableRef.git
```

Чтобы зафиксировать конкретную версию, добавь `#<тег>`, например `...Unity-StableRef.git#1.0.0`.

**Через `Packages/manifest.json`**

Добавь запись в блок `dependencies`:

```json
"com.egorshesterikov.stableref": "https://github.com/EgorShesterikov/Unity-StableRef.git"
```

**Через `.unitypackage`**

Скачай последний релиз со страницы [Releases](https://github.com/EgorShesterikov/Unity-StableRef/releases) и импортируй через `Assets → Import Package → Custom Package...`.

**Ручное копирование**

Склонируй или скачай репозиторий и скопируй папку куда угодно внутрь `Assets/` своего проекта.

**Требования:** Unity 2021.3 или новее.

## Проблема, которую это решает

Стандартный `[SerializeReference]` хранит полное assembly-qualified имя типа. Если переименовать или переместить класс, Unity теряет ссылку и поле становится `null`. `StableRef` разделяет сериализованную идентичность и имя класса — тип получает постоянный ID через `[StableTypeId]`, и можно переименовывать его свободно.

## Классы и атрибуты

| Тип | Назначение |
|---|---|
| `StableRef<T>` | Сериализуемая обёртка для одиночной полиморфной ссылки типа `T`. |
| `StableRefList<T>` | Сериализуемый список элементов `StableRef<T>`. |
| `[StableTypeId("id")]` | Присваивает классу постоянный ID. Переименовывай класс как угодно — Unity его найдёт. |
| `[StableRefCategory("Path")]` | Группирует тип под подменю в инспекторном селекторе. |
## Использование

### Объявление стабильного типа

```csharp
[Serializable]
[StableTypeId("my-package.damage-on-hit")]
[StableRefCategory("Combat")]
public class DamageOnHit : IEffect
{
    public int Amount;
}
```

Значение `[StableTypeId]` должно быть уникальным в проекте. Используй строки с неймспейсом, чтобы избежать коллизий.

### StableRef\<T\> в поле

```csharp
[Serializable]
public class ItemConfig : ScriptableObject
{
    public StableRef<IEffect> OnPickup;
}
```

### StableRefList\<T\>

```csharp
[Serializable]
public class AbilityConfig : ScriptableObject
{
    public StableRefList<IEffect> Effects;
}

// Перебор
foreach (var stableRef in config.Effects)
{
    var effect = stableRef?.Value;
    if (effect != null)
        effect.Apply();
}
```

<p align="center">
  <img src="Documentation~/inspector.gif" alt="Добавление типа через типизированный дропдаун" width="580">
</p>

## Автоматическая генерация ID

`[StableTypeId]` — опционален. Если атрибут не указан, StableRef автоматически использует **MonoScript GUID** (значение поля `guid` из `.meta`-файла скрипта) в качестве стабильного идентификатора. Это значит:

- **Переименование класса** — безопасно. GUID привязан к файлу, а не к имени класса.
- **Переименование или перемещение файла скрипта** — тоже безопасно. Meta-файл переезжает вместе с ассетом, GUID не меняется.
- **Удаление и повторное создание файла** — ссылка теряется (резолвится в `null`), но ошибка обрабатывается контролируемо. Проект продолжит работать; потерянный тип отобразится в Fix Missing Types.

Для типов, которые планируется активно рефакторить, явный `[StableTypeId]` надёжнее — он выживает даже при удалении и пересоздании файла скрипта.

## Инструменты редактора

Все инструменты доступны через **Tools → StableRef** в строке меню Unity.

**Find Usages** (`Tools/StableRef/Find Usages`) — сканирует префабы, открытые сцены и скриптовые объекты и показывает все места, где используется выбранный тип. Также доступно через правый клик на ассете скрипта: `Assets/Find StableRef Usages`.

<p align="center">
  <img src="Documentation~/find-usages.gif" alt="Окно Find Usages" width="640">
</p>

**Fix Missing Types** (`Tools/StableRef/Fix Missing Types`) — сканирует проект в поисках StableRef-полей, чей ID больше не соответствует ни одному известному типу. Удобно после рефакторинга — позволяет найти сломанные ссылки до того, как они превратятся в молчаливую потерю данных.

<p align="center">
  <img src="Documentation~/fix-missing.gif" alt="Окно Fix Missing Types" width="560">
</p>

## Лицензия

Распространяется под [MIT License](LICENSE.md).

Автор — **Egor Shesterikov**.
