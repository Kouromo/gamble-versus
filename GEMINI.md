# Projet Roll-Fight (Inspiré de "For The King") - Guide de Développement

Ce fichier sert de référence pour le développement du projet Unity "Roll-Fight" (M1 AMINJ). L'objectif est de créer un jeu abouti inspiré de *For The King*, intégrant une forte dimension aléatoire, tout en respectant scrupuleusement les grilles d'évaluation académiques (Externalisation des données et Structure de données).

## 🎯 Objectif Principal
Développer un prototype fonctionnel et visuellement attrayant d'un jeu de rôle / aventure avec des mécaniques aléatoires, destiné à enrichir un portfolio.

## 📋 Contraintes et Grilles d'Évaluation (Objectif 20/20)

### 1. Structure du Jeu (20 points)
*   **Aléatoire (4 pts) :** Le cœur du jeu. Doit inclure des choix aléatoires dans des listes (salles, ennemis, bonus, objets).
*   **Créativité et Originalité (4 pts) :** Héros asymétriques avec un système de combat basé sur la probabilité de réussite par attaque (`mainStat` et `rolls`).
*   **Graphismes et Level Design (4 pts) :** Utilisation de scènes de combat (arènes statiques pré-construites) avec placement de la caméra pour un rendu esthétique et maîtrisé.
*   **Extras et Polish (4 pts) :** Écrans de Titre/Fin, UI (Barres de vie, inventaire), Menu Pause, SFX/Musique.
*   **Document et Rendu (4 pts) :** Build Windows + Document PDF explicatif.

### 2. Externalisation des Données (Objectif 35/35 convertis sur 20)
L'architecture du code s'appuie massivement sur des fichiers JSON (`StreamingAssets`) et des `ScriptableObjects` (`EnemyDatabase`, `DecorationDatabase`, `ItemDatabase`) pour paramétrer le jeu sans toucher au code.

## 🏗️ Architecture et Conventions de Code

1.  **Data-Driven Design (Validé) :** Utilisation de `JsonUtility` et de classes wrapper (`RoundDataWrapper`, `PlayerDataWrapper`) pour lire les stats pures (PV, dégâts, précision `mainStat`, esquive `dodge`, couleur `colorIndex`).
2.  **Séparation Visuel / Logique (Validé) :** Les JSON ne contiennent que de la data. Les `ScriptableObjects` dans l'éditeur font le pont entre les noms textuels (ex: "bat") et les modèles 3D/Prefabs.
3.  **Level Design "Arène Statique" (Validé) :** Plutôt que de générer les murs aléatoirement, les arènes sont fixes dans la scène. Les ennemis et décorations "popent" aléatoirement ou selon les JSON sur des `Slots` (GameObjects vides pré-placés). La caméra se déplace vers l'arène active selon le Round.
4.  **Ne pas modifier les modèles 3D bruts :** Utiliser les FBX du dossier `Models` pour créer/mettre à jour des `Prefabs`.

## 📈 État d'Avancement (Journal)

- [x] **Phase 1 : Socle technique et Données (Externalisation)**
  - [x] Création des structures de données (`EntityData`, `PlayerData`, `EnemyData`, `RoundData`, `ItemData`).
  - [x] Création des fichiers de configuration JSON (`rounds.json`, `heroes.json`, `items.json`).
  - [x] Création du `DataManager` (Singleton) pour lire les fichiers au lancement.
  - [x] Création des ScriptableObjects (`EnemyDatabase`, `DecorationDatabase`, `ItemDatabase`) pour lier le texte aux Assets 3D.
  - [x] Équilibrage initial du système de combat (Rolls / MainStat / Dodge / Min-Max Damage).

- [x] **Phase 2 : Gestion de l'Environnement (Arènes Statiques)**
  - [x] Créer le script `LevelGenerator` qui lit un `RoundData` du `DataManager`.
  - [x] Déplacer la caméra sur l'arène physique correspondante.
  - [x] Instancier et nettoyer proprement les monstres et décorations sur les bons `Slots` de l'arène.
  - [x] Implémenter la navigation séquentielle (`LoadNextRound`).
  - [x] Préparer l'instanciation des héros (`HeroDatabase`, `heroSlots`).

- [x] **Phase 3 : Gameplay Core (Combat au Tour par Tour)**
  - [x] Implémenter le `TurnManager` (Ordre de passage basé sur la `speed` Joueurs / Ennemis).
  - [x] Implémenter la logique d'attaque (calcul des jets réussis via `mainStat`, esquive via `dodge`, et calcul des dégâts proportionnels).
  - [x] Implémenter l'utilisation d'objets (ex: Potion de soin).

- [x] **Phase 4 : UI et Polish (Extras & Polish)**
  - [x] Relier les statistiques du jeu à l'interface (Barres de vie, Logs de combat, Boutons d'Action).
  - [x] Créer l'écran de sélection de héros (affichant leur `description`) via `HeroSelectionUI` et `GameManager`.
  - [ ] Intégrer les menus (Titre validé, reste Pause, Fin) et le retour visuel (Sons, Particules).