# Redot release policy

Redot's release policy is in constant evolution. The description below
provides a general idea of what to expect, but what will actually
happen depends on the choices of core contributors and the needs of the
community at a given time.

## Redot versioning

Starting in 2026, Redot has moved to a new versioning scheme with a ``year.quarter`` system.
We will release a new version of Redot every quarter, or every 3 months. Starting with 26.1, 
Redot is hard forked from Godot 4.5.2, and aims to be compatible with most Godot 4.5.2 projects and extensions.


:::note

In addition to stable releases, we also periodically release beta versions.
These are not intended for production use, but we encourage users to try them out and provide feedback.
These are versioned slightly differently from stable releases, with a ``-beta.<Build Number>`` suffix (e.g. ``26.2-beta.1``)
 or ``-rc.<Build Number>`` suffix (e.g. ``26.2-rc.1``).

:::

## Release support timeline

Starting in 2026, Redot will be released on a quarterly basis. The current version is supported until the next quarterly release.

| **Version**           | **Release date** | **Support level** |
|-----------------------|------------------|-------------------|
| Redot 26.2 (`master`) | June 2026        | Unstable          |
| Redot 26.1            | Feburary 2026    | Supported         |
| Redot 4.4             | December 2025    | EOL               |
| Redot 4.3             | August 2024      | EOL               |

**Legend:**

|  **Status**  | **Description**          |
|:------------:|--------------------------|
|  Supported   | Full support             |
|     EOL      | No support (end of life) |
|   Unstable   | Development version      |

Pre-release Redot versions aren't intended to be used in production and are
provided for testing purposes only.

## Which version should I use for a new project?

We recommend using Redot 4.x for new projects, as the Redot 4.x series will be
supported long after 3.x stops receiving updates in the future. One caveat is
that a lot of third-party documentation hasn't been updated for Redot 4.x yet.
If you have to follow a tutorial designed for Redot 3.x, we recommend keeping
[doc_upgrading_to_Redot_4](doc_upgrading_to_Redot_4) open in a separate tab to check which methods
have been renamed (if you get a script error while trying to use a specific node
or method that was renamed in Redot 4.x).

If your project requires a feature that is missing in 4.x (such as GLES2/WebGL
1.0), you should use Redot 3.x for a new project instead.

## Should I upgrade my project to use new engine versions?

:::note

Upgrading software while working on a project is inherently risky, so
consider whether it's a good idea for your project before attempting an
upgrade. Also, make backups of your project or use version control to
prevent losing data in case the upgrade goes wrong.

That said, we do our best to keep minor and especially patch releases
compatible with existing projects.

:::

The general recommendation is to upgrade your project to follow new *patch*
releases, such as upgrading from 4.0.2 to 4.0.3. This ensures you get bug fixes,
security updates and platform support updates (which is especially important for
mobile platforms). You also get continued support, as only the last patch
release receives support on official community platforms.

For *minor* releases, you should determine whether it's a good idea to upgrade
on a case-by-case basis. We've made a lot of effort in making the upgrade
process as seamless as possible, but some breaking changes may be present in
minor releases, along with a greater risk of regressions. Some fixes included in
minor releases may also change a class' expected behavior as required to fix
some bugs. This is especially the case in classes marked as *experimental* in
the documentation.

*Major* releases bring a lot of new functionality, but they also remove
previously existing functionality and may raise hardware requirements. They also
require much more work to upgrade to compared to minor releases. As a result, we
recommend sticking with the major release you've started your project with if
you are happy with how your project currently works.

## When is the next release out?

While Redot contributors aren't working under any deadlines, we strive to
publish minor releases relatively frequently.

In particular, after the very length release cycle for 4.0, we are pivoting to
a faster paced development workflow, 4.1 released 4 months after 4.0, and 4.2
released 4 months after 4.1

Frequent minor releases will enable us to ship new features faster (possibly
as experimental), get user feedback quickly, and iterate to improve those
features and their usability. Likewise, the general user experience will be
improved more steadily with a faster path to the end users.

Maintenance (patch) releases are released as needed with potentially very
short development cycles, to provide users of the current stable branch with
the latest bug fixes for their production needs.

## What are the criteria for compatibility across engine versions?

:::note

This section is intended to be used by contributors to determine which
changes are safe for a given release. The list is not exhaustive; it only
outlines the most common situations encountered during Redot's development.

:::

The following changes are acceptable in patch releases:

- Fixing a bug in a way that has no major negative impact on most projects, such
  as a visual or physics bug. Redot's physics engine is not deterministic, so
  physics bug fixes are not considered to break compatibility. If fixing a bug
  has a negative impact that could impact a lot of projects, it should be made
  optional (e.g. using a project setting or separate method).
- Adding a new optional parameter to a method.
- Small-scale editor usability tweaks.

Note that we tend to be more conservative with the fixes we allow in each
subsequent patch release. For instance, 4.0.1 may receive more impactful fixes
than 4.0.4 would.

The following changes are acceptable in minor releases, but not patch releases:

- Significant new features.
- Renaming a method parameter. In C#, method parameters can be passed by name
  (but not in GDScript). As a result, this can break some projects that use C#.
- Deprecating a method, member variable, or class. This is done by adding a
  deprecated flag to its class reference, which will show up in the editor. When
  a method is marked as deprecated, it's slated to be removed in the next
  *major* release.
- Changes that affect the default project theme's visuals.
- Bug fixes which significantly change the behavior or the output, with the aim
  to meet user expectations better. In comparison, in patch releases, we may
  favor keeping a buggy behavior so we don't break existing projects which
  likely already rely on the bug or use a workaround.
- Performance optimizations that result in visual changes.

The following changes are considered **compatibility-breaking** and can only be
performed in a new major release:

- Renaming or removing a method, member variable, or class.
- Modifying a node's inheritance tree by making it inherit from a different class.
- Changing the default value of a project setting value in a way that affects existing
  projects. To only affect new projects, the project manager should write a
  modified ``project.Redot`` instead.

Since Redot 5.0 hasn't been branched off yet, we currently discourage making
compatibility-breaking changes of this kind.

:::note

When modifying a method's signature in any fashion (including adding an
optional parameter), a GDExtension compatibility method must be created.
This ensures that existing GDExtensions continue to work across patch and
minor releases, so that users don't have to recompile them.
See [doc_handling_compatibility_breakages](../Contributing/Development/handling_compatibility_breakages.md) for more information.

:::
