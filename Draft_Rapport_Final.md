# Projet Unity M1 AMINJ - Structure et Externalisation de Données
**Titre du jeu :** Roll-Fight
**Nom, Prénom :** [Ton Nom et Prénom]
**Date :** Mai 2026

---

## 1. Introduction & Note d'Intention

### Concept et Game Design
**Roll-Fight** est un prototype de jeu de rôle et d'aventure au tour par tour, fortement inspiré de jeux comme *For The King*. L'objectif principal de ce projet a été de concevoir une expérience de jeu centrée sur l'aléatoire et la probabilité, tout en construisant une architecture de code robuste, modulaire et "Data-Driven". 

Le cœur du gameplay repose sur un système de combat asymétrique où chaque action (attaque, esquive) est résolue par des jets de dés ("Rolls") basés sur les statistiques des personnages (`mainStat`, `dodge`). Cette mécanique de probabilité, au lieu de simples dégâts fixes, apporte une tension constante à chaque tour de jeu.

### Mise en Valeur
Pour valoriser ce projet, un soin tout particulier a été apporté au "Polish" et à l'expérience utilisateur (UX/UI). L'interface a été entièrement pensée pour être lisible et réactive : pop-ups de dégâts animés, affichage visuel des jets de dés en temps réel, barres de vie en "billboarding", et transitions de caméra fluides entre les différentes "arènes" de combat statiques. Les modèles 3D ont été intégrés dans des décors soignés pour créer une atmosphère immersive.

---

## 2. Contrôles du Jeu

Le jeu se veut intuitif et se joue intégralement à la souris :
*   **Clic Gauche :** Interagir avec l'interface (Sélection du héros, Boutons d'attaque, Utilisation d'objets : Potion de soin).
*   **Menu / Pause :** Touche `Echap` (ou bouton UI dédié) pour ouvrir le menu de pause et accéder aux options.

---

## 3. Architecture et Externalisation des Données (Objectif 35/35)

L'un des piliers de ce projet est l'externalisation massive des données. Le code a été pensé pour que le jeu soit paramétrable sans recompiler, via des fichiers externes.

*   **Data-Driven Design (Fichiers JSON) :** L'intégralité des statistiques des Héros, des Ennemis, des Objets, et la composition des Rounds (niveaux) sont définies dans des fichiers JSON placés dans le dossier `StreamingAssets` (ex: `heroes.json`, `rounds.json`). Un `DataManager` (Singleton) lit et parse ces données au lancement.
*   **ScriptableObjects :** Pour lier les données textuelles (JSON) aux éléments visuels (Modèles 3D, Prefabs, Icônes), j'ai utilisé des bases de données sous forme de `ScriptableObjects` (`EnemyDatabase`, `ItemDatabase`). Cela permet une séparation propre entre la *logique pure* et *l'affichage*.
*   **Paramètres de base :**
    *   **Son et Options :** Le volume est externalisé et sauvegardé via les `PlayerPrefs`. Il est rechargé automatiquement au démarrage du jeu.

---

## 4. Aléatoire, Créativité et Level Design (Objectif 20/20)

### Dimension Aléatoire
L'aléatoire est présent à tous les niveaux du jeu :
*   **Génération des combats :** L'ordre des salles et la composition des ennemis sont piochés aléatoirement à partir des listes définies dans les JSON. Les ennemis "popent" sur des emplacements (Slots) pré-définis de manière dynamique.
*   **Loot et Récompenses :** Les objets obtenus en fin de combat (ex: potions) sont tirés au sort.
*   **Mécaniques de Combat :** L'issue de chaque attaque dépend d'un calcul de probabilité complexe combinant les jets de dés (`rolls`), la statistique principale (`mainStat`) et les chances d'esquive de la cible (`dodge`).

### Level Design : Le choix des "Arènes Statiques"
Plutôt que de générer des salles procéduralement brique par brique, j'ai opté pour des **arènes statiques pré-construites**. Ce choix de Level Design permet un contrôle total sur l'esthétique, l'éclairage et le placement de la caméra pour chaque zone de combat, garantissant un rendu visuel de qualité, tout en conservant la génération procédurale pour le *contenu* de ces arènes (monstres et décors).

### Extras et Polish
Le jeu dispose d'un enrobage complet pour un rendu professionnel :
*   **Audio :** Un `AudioManager` gère dynamiquement les musiques (Menu vs Combat) et les effets sonores (SFX d'attaques, UI).
*   **Interface complète :** Écran Titre animé, sélection de héros avec prévisualisation 3D, menus d'options fonctionnels, et écrans de Fin / Game Over travaillés.
*   **Retours Visuels :** Particules d'impact, animations des modèles 3D, et textes flottants pour les dégâts.

---

## 5. Points Forts et Points Faibles

### Challenges Techniques Rencontrés (Points Forts)
*   **L'architecture Data-Driven :** La création du pont entre les données pures en JSON et l'instanciation physique des Prefabs dans Unity via les `ScriptableObjects` a été un défi majeur, mais extrêmement formateur. Cette architecture permet aujourd'hui d'ajouter un nouveau monstre ou un nouveau niveau en éditant simplement un fichier texte, sans écrire une seule ligne de code.
*   **Le Système de Combat au Tour par Tour :** Gérer l'ordre de passage (`TurnManager`) basé sur la statistique de vitesse, tout en synchronisant les animations, les particules, et les mises à jour d'interface (logs de combat, barres de vie) a nécessité l'utilisation de Coroutines et une gestion rigoureuse de l'état du jeu.

### Améliorations Possibles (Points Faibles)
*   **Sauvegarde complète de la progression :** Actuellement, seuls les paramètres sont sauvegardés. L'implémentation d'une sérialisation complète de l'état d'une partie (Héros, Inventaire, Round en cours) vers un fichier JSON de sauvegarde serait la prochaine étape logique.
*   **Plus de profondeur Pédagogique / Narrative :** Le scénario est actuellement très basique (enchaîner des combats jusqu'au Boss). Intégrer un système de dialogues ou de quêtes externalisé (pour valider la section "Pédagogie" de la grille d'externalisation) enrichirait grandement l'expérience.

---

## 6. Captures d'écran de code (À insérer)

*(Note pour la rédaction : Insère ici 2 ou 3 captures d'écran de code dont tu es fier. Voici des suggestions basées sur ton projet :)*

1.  **Capture du `DataManager` ou des classes de désérialisation JSON :** Pour prouver l'externalisation.
2.  **Capture de la méthode de calcul d'attaque (`RollAttack` ou équivalent) :** Pour montrer l'algorithme probabiliste basé sur les statistiques.
3.  **Capture du `LevelGenerator` :** Pour illustrer comment tu instancies les ennemis sur les "Slots" à partir des données du Round.
