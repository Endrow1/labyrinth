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
