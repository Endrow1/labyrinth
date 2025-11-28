# Rapport Étape 1 - Extension de Inventory

----

L’objectif de cette étape était de modifier la classe Inventory afin qu’elle puisse gérer une collection d’items au lieu d’un seul. 
Ce changement a nécessité un léger refactoring ainsi qu’une mise à jour des tests unitaires.

## Inventory.cs
La classe a été adaptée pour intégrer et manipuler une collection d’objets. Les méthodes existantes ont été ajustées en conséquence afin de conserver le comportement attendu tout en permettant la gestion de plusieurs items. 

```C#
protected ICollectable? _item = item;
```
Devient :
```C#
protected List<ICollectable>? _items = item is not null ? [item] : [];
```

Pour cette partie Rider est très utile car il permet de renommer automatiquement les variables et méthodes associées. L'ensemble des références dans le projet sont mis à jours automatiquement.

## Door.cs
La classe Door a dû être modifié car elle utilise un son propre inventory pour gérer les clés qui peuvent être rentrée dans la porte. Ce qu'on fait c'est que maintenant le RandExplorer regarde si son premier objet est une clé grâce à `.First`, ensuite l'inventaire est passé à la porte qui récupère par défaut le premier élément. Par la suite le mécanisme ne change pas car la porte ne gère qu'une clé à la fois.

## Tests 
Après modification des tests pour adapter les changements sans en modifier le comportement tous les tests passent --> On peut passer à la prochaine étape.

---------

# Partie 2 - Ajout

Auparavant on avait une classe Keymaster qui gérait la relation entre une porte et une salle à clé. Cependant la logique était limitée car elle supposait que la génération des portes et des salles à clé se faisait dans un ordre spécifique (une salle à clé suivie de sa porte correspondante).
L'objectif était d'améliorer la classe Keymaster pour qu’elle puisse gérer n’importe quelle distribution de portes (doors) et de salles à clé (key rooms), peu importe l’ordre dans lequel ils sont créés.


## Keymaster.cs


La version initiale de Keymaster ne supportait pas toutes les permutations possibles de :

- Plusieurs portes créées avant leurs salles,
- Plusieurs salles créées avant les portes,
- alternances complexes (ex : porte -> salle -> porte -> porte -> salle…).

Pour corriger cela, l'implémentation a été généralisée en utilisant deux **queues FIFO** :

- `_unplacedKeys` : inventaires de clés associés aux portes créées,
- `_pendingKeyRooms` : salles en attente de recevoir une clé.

La méthode interne `PlaceKey()` assure l’association immédiate d'une clé à une salle **dès que les deux files contiennent au moins un élément**.

Donc :
- l’ordre de correspondance porte/clé,
- la cohérence pour toutes les séquences possibles de création.

Enfin, la méthode `Dispose()` vérifie qu'il ne reste **aucune clé non placée** ni **salle vide** dans nos deux files, assurant l’intégrité complète de la configuration du labyrinthe (comme une porte créée sans sa clé).

---

## Mise à jour et ajout des tests

Les tests existants ont été complétés pour couvrir tous les scénarios typiques :

### **Cas vérifiés**

- **Door -> Room** : la clé est placée automatiquement après la création d’une salle.
- **Plusieurs portes puis plusieurs salles** : respect strict de l'ordre de création.
- **Plusieurs salles puis plusieurs portes** : même cohérence que ci-dessus.
- **Ordre mixte** (cas complexe) : validation de la robustesse et de la correspondance FIFO.
- **Détection d’erreurs** : `Dispose()` doit lever une exception s’il reste des portes ou des clés non appariés dans le labyrinthe.

Les tests garantissent :

- l’ouverture correcte de chaque porte avec sa salle correspondante,
- l’absence de fuite de clés,
- l’absence de salle laissée vide.

Pour créer un fichier de test propre et organisé, j'ai divisé certaines partie du tests redondante en méthode privée réutilisable dans la classe de test (ex: création des rooms, passage des portes avec les clés pour vérifier qu'elles s'ouvrent correctement, etc...).

Il y à cependant le cas du Dispose où j'ai pas vraiment pris de décision, à la fin du test le KeyMaster appel Dispose automatiquement, donc en cas d'erreur le test échoue automatiquement. J'ai donc laissé comme ça. Peut-être qu'englober tous les tests autour d'un bloc Assert.DoesNotThrow serait plus propre, mais ça alourdirait les tests.

### Validation finale

Après correction de l’algorithme et mise à jour des tests :

**Tous les tests passent avec succès**, démontrant que la classe supporte désormais toutes les permutations possibles de création.

On peut passer à la suite

----------

