# SCIMClient
An open-source implementation of a SCIM V2 client that is fully compatible with Entra ID. This library works with both users and groups. A Swagger front-end and a very simple demo repository class is included. It also works with the standard enterprise extensions in Entra ID. No authorization for the endpoints is included, and **you need to manually add this to use the library in production**. The reason for not simply including this, is that there are different ways to do this and that I hope to avoid major compatibility issues. For a detailed description of how the app works, **see the wiki**.

This software is 100%free without ifs or buts under the MIT license. [If you like it, feel free to buy me a coffee.](buymeacoffee.com/Tobiesen)

## What is SCIM?

SCIM (System for Cross-domain Identity Management) is an open standard protocol designed to simplify user identity management across multiple systems and applications. It enables automated provisioning, updating, and deprovisioning of user accounts by synchronizing identity information between identity providers (IdPs) and service providers (SPs). SCIM uses REST APIs and JSON-based payloads to facilitate seamless communication and data exchange. By implementing SCIM, organizations can reduce administrative overhead, improve security, and ensure compliance with identity governance policies. It is widely adopted by cloud-based applications and enterprise systems to streamline identity lifecycle management. Note that SCIM is
an open protocol, **it is not a framework or a plugin** in it's own right.

## Who uses SCIM?

SCIM is used by Azure Entra ID, Slack, Dropbox, Octa and many others. This library is tuned for Entra ID, but with a few, very simple modifications, it should work just fine on any service using SCIM V2 for user provisioning. This library is also designed to work without Graph API or similar third party tools to avoid tight-coupling to the extent that's possible.
