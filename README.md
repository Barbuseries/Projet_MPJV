# Membre d'équipe
- Edorh François (EDOF19059507)
- Guison Vianney (GUIV30069402)
- Klug David (KLUD13059501)


# Implémentations
## Transformations (CustomTransform)
- Translation (dans le repère du monde ou de l'objet)
- Rotation (axe-angle, euler)
- Mise à l'échelle (axe, facteurs suivant x, y et z)
- Projection (axe)
- Réflexion (axe)

## Dynamique linéaire (CustomRigidBody)
- Masse, vitesse, accéleration
- Forces
- Gravité
- "Frottements" (damping)


## Dynamique rotationnelle (CustomRigidBody, Shape)
- Vitesse angulaire, accélération angulaire
- Forces (en un point)
- Torque
- Inertie (suivant la forme du Rigidbody: Cube, ellispoid ou sphère)
- "Frottements" (damping)

## Collisions (CustomCollider)
- Avec l'ensemble des entités pouvant entrer en collision (liste dans
  CustomGameWorld)
- Ne prend en compte que la partie linéaire (n'occasionne pas de
  rotation)
  - Une tentative a été faite mais ne fonctionne pas dans tous les cas
- Collision sphère - sphère et sphère - boîte prenne en compte la
  rotation pour calculer la collision
- Collison boîte - boîte ne prend pas en compte la rotation pour
  calculer la rotation
- Méthode
  - Repérer la collision (méthode différente entre chaque type de
    CustomCollider)
  - Calculer l'impulsion à donner aux deux entités
    (proportionnellement à leur masse)
  - Déplacer manuellement les entités si elles s'interpénètrent
    (proportionnellement à leur masse)

## Contraintes (CustomJoint)
- Ressort (CustomSpringJoint)
  - Considère l'écart initial entre les deux entités comme position de
    repos
  - Applique une force sur les deux entités, proportionnelle à leur
    distance par rapport à cette position de repos


# Démos
## Billards
Contrôle, à la souris, d'un boîte reliée par un ressort à un point fixe pouvant entrer en collision avec les boules placées sur le terrain.

## BallShower
Un cube, légèrement penché et sujet à la gravité, est relié par des ressorts à quatre sphères immobiles. 
Un clique gauche crée des sphères à l'emplacement de la souris, un clic droit crée des cubes.


---
Version 2017 d'Unity
