# SCIM API
An open-source implementation of a SCIM V2 client that is fully compatible with Entra ID. This library works with both users and groups. A Swagger front-end and a very simple demo app is included. The client also works with the standard enterprise extensions in Entra ID.  For a detailed description of how the app works, **see the wiki**.

This software is 100% free without ifs or buts under the MIT license. [If you like it, feel free to buy me a coffee.](https://buymeacoffee.com/Tobiesen)

## What is SCIM?

SCIM (System for Cross-domain Identity Management) is an open standard protocol designed to simplify user identity management across multiple systems and applications. It enables automated provisioning, updating, and deprovisioning of user accounts by synchronizing identity information between identity providers (IdPs) and service providers (SPs). SCIM uses REST APIs and JSON-based payloads to facilitate seamless communication and data exchange. By implementing SCIM, organizations can reduce administrative overhead, improve security, and ensure compliance with identity governance policies. It is widely adopted by cloud-based applications and enterprise systems to streamline identity lifecycle management. Note that SCIM is
an open protocol, **it is not a framework or a plugin** in it's own right.

## Who uses SCIM?

SCIM is used by Azure Entra ID, Slack, Dropbox, Octa and many others. This library is tuned for Entra ID, but with a few, very simple modifications, it should work just fine on any service using SCIM V2 for user provisioning. This library is also designed to work without Graph API or similar third party tools to avoid tight-coupling to the extent that's possible.

## Architecture

The library is a standard MVC pattern using a 3-tier layout. It employs services and a repository at the bottom. Domain models are handled directly from the service. While a repository isn't strictly necessary when using a document database, it will ensure that the library is not tight-coupled to any database system, but that it can work with anything. A good and free alternative is MongoDB.

The API relies heavily on dependency injection. Modularity and ease of use has taken precedence over speed in this project.

## Testing it locally

The demo client comes with a simple Swagger front-end. If you want to test it locally from Azure Entra ID in your web browser, you can also download NGrok. This will allow you to create a tunnel to your local service that Entra ID can use. [The documentation for this is fairly straightforward.](https://ngrok.com/docs/getting-started/)

![image](https://github.com/user-attachments/assets/a2f55159-df4b-4424-a1ad-7ea2541bd3a2)

Entra ID by default uses the following verbs: **GET**, **POST** and **PATCH** and **DELETE**. The latter is only used for groups, while the three first verbs are used for both users and groups. **PUT** is not used by Entra ID. Deleted or deactivated usersare handled using an "Active" flag and a PATCH request instead instead. A delete endpoint is included in this API to allow you to hard delete an entry from a custom front-end, Swagger or Postman.

#### JSONs for testing various API calls can (soon) be found in the wiki for this project.

(c) Ole Tobiesen
