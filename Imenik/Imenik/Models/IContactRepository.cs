using Imenik.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imenik.Entities
{
    public interface IContactRepository
    {

        /// <summary>
        ///     Only to make deleting easier, not loading contact information.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Owner GetOwnerByUsername(string username);
        int GetOwnerId(Owner owner);

        /// <summary>
        ///     Retrieve all registered users on service
        /// </summary>
        /// <returns></returns>
        IQueryable<Owner> GetOwners();
        /// <summary>
        ///     Retrieve all contacts of owner with the specified user name
        /// </summary>
        /// <param name="userName" type="System.String">Owner's user name.</param>
        /// <returns></returns>
        IQueryable<Contact> GetContacts(string userName);

        /// <summary>
        ///     Retrieve all contacts of owner with the specified owner ID
        /// </summary>
        /// <param name="userId" type="int">Id of the owner</param>
        /// <returns></returns>
        IQueryable<Contact> GetContacts(int userId);


        /// <summary>
        ///     Retrieve the information for contact with specified ID
        /// </summary>
        /// <param name="contactId" type="int">Identifier of the contact</param>
        /// <returns>Adresar.Data.Contact</returns>
        Contact GetContact(int contactId);

        /// <summary>
        ///     Get the phone by phone number.
        /// </summary>
        /// <param name="phonenumber"></param>
        /// <returns></returns>
        Phone GetPhone(string phonenumber);

        Phone GetPhone(int phoneid);

        /// <summary>
        ///     Create new owner.
        /// </summary>
        /// <param name="owner" type="Adresar.Data.Owner">
        ///     Owner to be added to the database
        /// </param>
        /// <returns type="bool">
        ///     true if the Owner is added, false otherwise
        /// </returns>        
        bool Create(Owner owner);
        /// <summary>
        ///     Create a phone and put it in DB
        /// </summary>
        /// <param name="phone" type="Adresar.Data.Phone"></param>
        /// <returns type="bool">true if the phone is added, false otherwise</returns>
        bool Create(Phone phone);
        
        /// <summary>
        ///     Create new contact and add it to the owner with specified user name
        /// </summary>
        /// <param name="userName" type="System.String">
        ///     User name of the owner. 
        /// </param>
        /// <param name="contact" type="Adresar.Data.Contact">Contact to be added in the userName's phonebook.</param>
        /// <returns type="bool">true if the contact is added, false otherwise</returns>
        bool Create(string userName, Contact contact);
        /// <summary>
        ///     Create the phone and add it to the contact
        /// </summary>
        /// <param name="contactId" type="int">Identifier of the contact whom the phone number belongs to</param>
        /// <param name="phone" type="Adresar.Data.Phone">Phone number that should be added.</param>
        /// <returns type="bool">true if the phone number was added, false otherwise.</returns>
        bool Create(int contactId, Phone phone);

        /// <summary>
        ///     Update contact information
        /// </summary>
        /// <param name="newContact" type="Imenik.Entities.Contact">
        ///     New contact information
        /// </param>
        /// <returns type="bool">true if the contact is updated, false otherwise</returns>
        bool Update(Contact newContact);
        /// <summary>
        ///     Update informations about the phone
        /// </summary>
        /// <param name="newPhone" type="Imenik.Entities.Phone">New phone number</param>
        /// <returns type="bool">true if the phone number is updated, false otherwise</returns>
        bool Update(Phone newPhone);
        /// <summary>
        ///     Delete the owner. Deletes all the contact informations belonging to the contact as well.
        /// </summary>
        /// <param name="id" type="bool">true if the owner was deleted, false otherwise</param>
        /// <returns></returns>
        bool DeleteOwner(int id);
        /// <summary>
        ///     Delete contact from the table
        /// </summary>
        /// <param name="id" type="int">Identifier of the contact to be deleted</param>
        /// <returns>true if the contact is deleted, false otherwise</returns>
        bool DeleteContact(int id);
        /// <summary>
        ///     Delete the specified phone information from the database
        /// </summary>
        /// <param name="id" type="int">Identifier of the phone</param>
        /// <returns>true if the phone is deleted, false otherwise</returns>
        bool DeletePhone(int id);

        /// <summary>
        ///     Look in the database to see if the specified phone number is stored. 
        /// </summary>
        /// <param name="phonenumber"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        bool CheckForPhone(string phonenumber, int id);
        
        /// <summary>
        ///     Save changes to the database
        /// </summary>
        /// <returns>true if the changes were successfully</returns>
        bool Save();


    }
}
