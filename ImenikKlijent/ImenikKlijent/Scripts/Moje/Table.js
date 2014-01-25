// Svi kontakti
var ALLCONTACTS;
// Trenutna stranica i broj kontakata na stranici
var currentPage = 0;
var pageLimit = 7;

var defaultAction;

// Pomoćna funkcija za selekciju kontakata
Array.prototype.select = function (howMany, skip) {
    var lowerLimit = skip * howMany;
    var upperLimit = (skip + 1) * howMany;
    if (this.length < upperLimit)
        upperLimit = this.length;
    var ret = [];
    for (var i = lowerLimit; i < upperLimit; ++i)
    {
        ret.push(this[i]);
    }
    return ret;
}

// Brisanje elementa iz polja
Array.prototype.delete = function (id) {
    var ret = [];
    for (var i = 0; i < this.length; ++i) {
        if (this[i].phoneId != id)
            ret.push(this[i]);
    }
    return ret;
}


function printTable(data) {
    var tableBody = $("#content");
    var html = "";
    for (var i = 0; i < data.length; ++i) {
        var contact = data[i];
        html = html + "<tr id='contact" + contact.contactId + "'>";
        html = html + "<td>" + contact.firstName + "</td>";
        html = html + "<td>" + contact.lastName + "</td>";
        html = html + "<td>" + contact.city + "</td>";
        html = html + "<td>";
        if (contact.phones != null) {
            for (var j = 0; j < contact.phones.length; ++j) {
                html = html + contact.phones[j].phoneNumber + "; ";
            }
        }
        html = html + "</td>";
        
        // Sada CRUD dio tablice
        html = html + "<td class='crud'><button id='details" + contact.contactId + "'>Detalji</button>";
        html = html + "<button id='edit" + contact.contactId + "'>Uredi</button>";
        html = html + "<button id='delete" + contact.contactId + "'>Obriši</button></td>";
        html = html + "</tr>";
    }
    tableBody.html(html);
}

// Postavi pager da bude kak treba
function manglePager(length, page) {
    if (page == 0)
        $("#prev").addClass("hidden");
    else {
        if ($("#prev").hasClass("hidden"))
            $("#prev").removeClass("hidden");        
    }
    var maxPages = Math.floor(length / pageLimit);
    if (maxPages <= page)
        $("#next").addClass("hidden");
    else {
        if ($("#next").hasClass("hidden"))
            $("#next").removeClass("hidden");

    }
}


function TD(what) {
    var ret = "<td>" + what + "</td>";
    return ret;
}

function personalTableHTML(model) {
    var html = "<tr><td><strong>Ime</strong></td>";
    html = html + TD(model.firstName);
    html = html + "</tr>";
    html = html + "<tr><td><strong>Prezime</strong></td>";
    html = html + TD(model.lastName);
    html = html + "</tr>";
    html = html + "<tr><td><strong>Grad</strong></td>";
    html = html + TD(model.city);
    html = html + "</tr>";
    html = html + "<tr><td><strong>Opis</strong></td>";
    html = html + TD(model.description);
    html = html + "</tr>";
    
    return html;
}


function addPhone(phone) {
    var html = "";
    html = html + "<tr id='" + phone.phoneId + "'><td>" + phone.type + "</td>";
    html = html + "<td>" + phone.phoneNumber + "</td>";
    html = html + "<td>" + phone.description + "</td>";
    html = html + "<td><button id='editPhone" + phone.phoneId + "'>Uredi</button>";
    html = html + "<button id='deletePhone" + phone.phoneId + "'>Obriši</button></td></tr>";
    return html;
}


function createNewPhoneRow() {
    return "<tr id='newRow'><td><button id='addNewPhone'>Dodaj broj</button></td><td></td><td></td><td></td></tr>";
}

// Provjeri da li je moguće dodat novi broj
function canAdd(number) {
    number = number.replace(/\s/g, '');
    for (var i = 0; i < ALLCONTACTS.length; ++i) {
        if (ALLCONTACTS[i].phones != null) {
            for (var j = 0; j < ALLCONTACTS[i].phones.length; ++j) {
                if (ALLCONTACTS[i].phones[j].phoneNumber == number)
                    return false;
            }
        }
    }
    return true;
}

function canModify(number, index) {
    number = number.replace(/\s/g, '');
    for (var i = 0; i < ALLCONTACTS.length; ++i) {
        if (ALLCONTACTS[i].phones != null) {
            for (var j = 0; j < ALLCONTACTS[i].phones.length; ++j) {
                if (ALLCONTACTS[i].phones[j].phoneNumber == number) {
                    if (i != index)
                        return false;
                }
            }
        }
    }
    return true;
}

function addNewPhoneHandler(model) {
    // Sad dodat input za novi kontakt
    var html = "<tr id='inputNewPhone'><td><input type='text' placeholder='Tip broja' id='newType'/></td>";
    html = html + "<td><input type='text' placeholder='Broj' id='newPhone'/></td>";
    html = html + "<td><input type='text' placeholder='Opis' id='newDescription'/></td>";
    html = html + "<td><button id='save'>Spremi</button><button id='cancel'>Odustani</button></td></tr>"
    $("#newRow").remove();
    var contactPhones = $("#contactPhones");
    contactPhones.append(html);

    $("#cancel").on("click", function () {
        $("#inputNewPhone").remove();
        contactPhones.append(createNewPhoneRow());
        $("#addNewPhone").on("click", addNewPhoneHandler);
    });

    $("#save").on("click", function () {
        var _type = $("#newType").val();
        var _phoneNumber = $("#newPhone").val();

        if (canAdd(_phoneNumber)) {
            var _description = $("#newDescription").val();
            var phone = {
                phoneNumber: _phoneNumber,
                type: _type,
                description: _description
            };
            $.post(window.location.origin + "/home/postphone/" + $("#hiddenId").val(), phone, function (data) {
                if (data.phoneId == 0)
                    alert("Nešto je pošlo po zlu.  Nisam uspio spremiti ovaj broj. Javi mi što i kad se dogodilo.");
                else {
                    if (model.phones == null)
                        model.phones = [];
                    model.phones.push(data);
                    for (var i = 0; i < ALLCONTACTS.length; ++i) {
                        if (ALLCONTACTS[i].contactId == model.contactId)
                        {
                            ALLCONTACTS[i].phones = model.phones;
                            break;
                        }
                    }
                    handleDetails(model);
                }});
        }
        else {
            alert("Već imaš taj broj spremljen, potraži ga.");
        }
    });
}

function printPhones(contactPhones, model) {
    if (model.phones == null) {
        html = createNewPhoneRow();
        contactPhones.html(html);
    }
    else {
        html = "";
        for (var i = 0; i < model.phones.length; ++i) {
            html = html + addPhone(model.phones[i]);
        }
        html = html + createNewPhoneRow();
        contactPhones.html(html);
    }
}


function handleDelete(contactPhones, model) {
    $("[id^=deletePhone]").each(function () {
        $(this).on("click", function () {
            var id = $(this).attr("id").slice(11, $(this).attr("id").length);
            $.post(window.location.origin + "/home/deletephone/" + id, "", function (data, status, xhr) {
                if (xhr.status == 200) {
                    model.phones = model.phones.delete(id);
                    for (var i = 0; i < ALLCONTACTS.length; ++i)
                    {
                        if (ALLCONTACTS[i].contactId == model.contactId)
                        {
                            ALLCONTACTS[i].phones.delete(id);
                            break;
                        }
                    }
                    handleDetails(model);
                }
                else {
                    alert("Oops, nešto ne valja. Malo me sramota sad. Obavijesti me o ovome.");
                }
            });
        });
    });
}

function handleEdit(contactPhones, model) {
    $("[id^=editPhone]").each(function () {
        $(this).on("click", function () {
            $("#addNewPhone").toggleClass("hidden");
            var id = $(this).attr("id").slice(9, $(this).attr("id").length);
            var phone, 
                currind;
            for (var i = 0; i < model.phones.length; ++i) {
                if (model.phones[i].phoneId == id) {
                    phone = model.phones[i];
                    currind = i;
                    break;
                }
            }

            var phoneNumber = phone.phoneNumber;
            var html = "";
            html = html + "<td><input type='text' id='newType' value='" + phone.type + "'/></td>";
            html = html + "<td><input type='text' id='newPhone' value='" + phone.phoneNumber + "'/></td>";
            html = html + "<td><input type='text' id='newDescription' value='" + phone.description + "'/></td>";
            html = html + "<td><button id='save'>Spremi</button>";
            html = html + "<button id='cancel'>Odustani</button></td>";

            $("[id='" + id + "'").html(html);

            $("#cancel").on("click", function () {
                handleDetails(model);
            });
            $("#save").on("click", function () {
                var newPhone = {
                    type: $("#newType").val(),
                    phoneNumber: $("#newPhone").val(),
                    description: $("#newDescription").val(),
                    phoneId:id
                };
                if (canModify(newPhone.phoneNumber, currind)) {
                    $.post(window.location.origin + "/home/editphone?old=" + phoneNumber, newPhone, function (data, status, xhr) {
                        if (xhr.status == 200) {
                            model.phones[currind] = data;
                            for (var j = 0; j < ALLCONTACTS.length; ++j) {
                                if (ALLCONTACTS[j].contactId == model.contactId) {
                                    ALLCONTACTS[j].phones[currind] = data;
                                    break;
                                }
                            }
                            handleDetails(model);
                        }
                    });
                }
                else {
                    alert("Ne mogu dodijeliti ovaj broj zadanom kontaktu. Već si ga registrirao.");
                }
            });

        });
    });
}



// Puni sadržaj sectiona contactDetails, prikazuje ga i skriva tablicu
function handleDetails(model) {
    $("#hiddenId").val(model.contactId);
    // Prvo ćopim tablicu i popunim joj retke o osobnim podacima
    var table = $("#personal").html(personalTableHTML(model));
    $("#contacts").addClass("hidden");
    $("#contactDetails").removeClass("hidden");

    // Zatim strpam sliku gdje joj je mjesto
    var html = "<img src='" + model.imgUrl + "' width='250' height='150' />";
    $("#image").html(html);

    // A sad manipulacija kontakt telefonima
    var contactPhones = $("#contactPhones");
    printPhones(contactPhones, model);

    // OK, ovo sada dodaje novi broj uredno
    $("#addNewPhone").on("click", function () { addNewPhoneHandler(model); });
    handleDelete(contactPhones, model);
    handleEdit(contactPhones, model);
}



function startPage() {
    var viewData = ALLCONTACTS.select(pageLimit, currentPage);
    printTable(viewData);
    manglePager(ALLCONTACTS.length, currentPage);


    $("[id^='details']").each(function () {
        $(this).on("click", function () {
            var contactId = $(this).attr("id").slice(7, $(this).attr("id").length);
            var model;
            for (var i = 0; i < viewData.length; ++i) {
                if (viewData[i].contactId == contactId)
                    model = viewData[i];
            }
            handleDetails(model);
        });
    });


    $("[id^='edit']").each(function () {
        $(this).on("click", function () {
            if ($("#postwrap").hasClass("hidden"))
                $("#showHide").trigger("click");
            var id = $(this).attr("id").slice(4, $(this).attr("id").length);
            var action = window.location.origin + "/home/putcontact/" + id;
            $("#postContact").attr("action", action);
            // Dohvat kontakta za update
            var model;
            for (var i = 0; i < viewData.length; ++i) {
                if (viewData[i].contactId == id)
                    model = viewData[i];                
            }
            
            $("[name='firstName']").val(model.firstName);
            $("[name='lastName']").val(model.lastName);
            $("[name='city']").val(model.city);
            $("[name='description']").val(model.description);
            $("#miniImg").attr("src", model.imgUrl);

        });
    });

    $("[id^='delete']").each(function () {
        $(this).on("click", function () {
            var id = $(this).attr("id").slice(6, $(this).attr("id").length);
            if (confirm("Zbilja želiš u potpunosti ukloniti ovaj kontakt?")) {
                $.post(window.location.origin + "/home/deletecontact/" + id, "", function (data, status, xhr) {
                    // Mogao bih se malo više potrudit i ovakve izmjene učiniti lokalno, a kasnije ih uskladiti sa serverom
                    window.location.reload();
                });
            }
            else {
                startPage();
            }
        });
    });
}

function compareFName(a, b) {
    if (a.firstName < b.firstName)
        return -1;
    if (a.firstName > b.firstName)
        return 1;
    return 0;
}

function compareLName(a, b) {
    if (a.lastName < b.lastName)
        return -1;
    if (a.lastName > b.lastName)
        return 1;
    return 0;
}

function compareCity(a, b) {
    if (a.city < b.city)
        return -1;
    if (a.city > b.city)
        return 1;
    return 0;
}

$("document").ready(function () {
    defaultAction = $("#postContact").attr("action");
    var url = window.location.origin + "/home/getcontacts";
    $.post(url, "", function (data, status, xhr) {
        ALLCONTACTS = data.contacts;
        startPage();
    }, "json");

    $("#prev").on("click", function () {
        currentPage = currentPage - 1;
        startPage();
    });

    $("#next").on("click", function () {
        currentPage = currentPage + 1;
        startPage();
    });

    $("#contactDetails").addClass("hidden");
    $("#backButton").on("click", function () {
        $("#contacts").removeClass("hidden");
        $("#contactDetails").addClass("hidden");
        startPage();
    });

    $("#showHide").on("click", function () {
        $("#postwrap").toggleClass("hidden");
        $("#postContact").attr("action", defaultAction);
        $("[name='firstName']").val("");
        $("[name='lastName']").val("");
        $("[name='city']").val("");
        $("[name='description']").val("");
        $("#miniImg").attr("src", "");


        if ($(this).text() == "Dodaj novog kontakta")
            $(this).text("Sakrij");
        else
            $(this).text("Dodaj novog kontakta");
    });

    $("#sortIme").on("click", function () {
        // "&#9660;" dolje
        // "&#9650;" gore
        if ($("#imeSmjer").html() == "down") {
            ALLCONTACTS.sort(compareFName);
            $(this).html("&#9660;");
            $("#imeSmjer").text("up");
            startPage();
        }
        else 
        {
            ALLCONTACTS.sort(compareFName);
            ALLCONTACTS.reverse();
            $(this).html("&#9650;");
            $("#imeSmjer").text("down");
            startPage();
        }
    });


    $("#sortPrezime").on("click", function () {
        // "&#9660;" dolje
        // "&#9650;" gore
        if ($("#prezimeSmjer").html() == "down") {
            ALLCONTACTS.sort(compareLName);
            $(this).html("&#9660;");
            $("#prezimeSmjer").text("up");
            startPage();
        }
        else {
            ALLCONTACTS.sort(compareLName);
            ALLCONTACTS.reverse();
            $(this).html("&#9650;");
            $("#prezimeSmjer").text("down");
            startPage();
        }
    });
    $("#sortCity").on("click", function () {
        // "&#9660;" dolje
        // "&#9650;" gore
        if ($("#citySmjer").html() == "down") {
            ALLCONTACTS.sort(compareCity);
            $(this).html("&#9660;");
            $("#citySmjer").text("up");
            startPage();
        }
        else {
            ALLCONTACTS.sort(compareCity);
            ALLCONTACTS.reverse();
            $(this).html("&#9650;");
            $("#citySmjer").text("down");
            startPage();
        }
    });

    $("#showHideFilter").on("click", function () {
        $("#filterSection").toggleClass("hidden");
        $("#deactivate").addClass("hidden");
    });

    $("#activate").on("click", function () {
        var choice = $("select").val();
        if (choice == "fName") {
            var query = $("#query").val();
            if (query == "")
                return;
            var tmpList = ALLCONTACTS;
            var incomplete = [];
            ALLCONTACTS = [];
            for (var i = 0; i < tmpList.length; ++i)
            {
                if (tmpList[i].firstName == query || tmpList[i].firstName.toLowerCase() == query.toLowerCase())
                    ALLCONTACTS.push(tmpList[i]);
                else if (tmpList[i].firstName.toLowerCase().search(query.toLowerCase()) >= 0)
                    incomplete.push(tmpList[i]);
            }
            ALLCONTACTS = ALLCONTACTS.concat(incomplete);
            // Sada su tu učitani svi kontakti koji zadovoljavaju uvjet
            // Gumb za povratak u normalu
            $("#deactivate").removeClass("hidden");
            $("#deactivate").on("click", function () {
                ALLCONTACTS = tmpList;
                currentPage = 0;
                $("#showHideFilter").trigger("click");
                startPage();
            });
            
            currentPage = 0;
            startPage();
        }

        if (choice == "lName") {
            var query = $("#query").val();
            if (query == "")
                return;
            var tmpList = ALLCONTACTS;
            var incomplete = [];
            ALLCONTACTS = [];
            for (var i = 0; i < tmpList.length; ++i) {
                if (tmpList[i].lastName == query || tmpList[i].lastName.toLowerCase() == query.toLowerCase())
                    ALLCONTACTS.push(tmpList[i]);
                else if (tmpList[i].lastName.toLowerCase().search(query.toLowerCase()) >= 0)
                    incomplete.push(tmpList[i]);
            }
            ALLCONTACTS = ALLCONTACTS.concat(incomplete);
            // Sada su tu učitani svi kontakti koji zadovoljavaju uvjet
            // Gumb za povratak u normalu
            $("#deactivate").removeClass("hidden");
            $("#deactivate").on("click", function () {
                ALLCONTACTS = tmpList;
                currentPage = 0;
                $("#showHideFilter").trigger("click");
                startPage();
            });

            currentPage = 0;
            startPage();
        }

        if (choice == "city") {
            var query = $("#query").val();
            if (query == "")
                return;
            var tmpList = ALLCONTACTS;
            var incomplete = [];
            ALLCONTACTS = [];
            for (var i = 0; i < tmpList.length; ++i) {
                if (tmpList[i].city == query || tmpList[i].city.toLowerCase() == query.toLowerCase())
                    ALLCONTACTS.push(tmpList[i]);
                else if (tmpList[i].city.toLowerCase().search(query.toLowerCase()) >= 0)
                    incomplete.push(tmpList[i]);
            }
            ALLCONTACTS = ALLCONTACTS.concat(incomplete);
            // Sada su tu učitani svi kontakti koji zadovoljavaju uvjet
            // Gumb za povratak u normalu
            $("#deactivate").removeClass("hidden");
            $("#deactivate").on("click", function () {
                ALLCONTACTS = tmpList;
                currentPage = 0;
                $("#showHideFilter").trigger("click");
                startPage();
            });

            currentPage = 0;
            startPage();
        }

        if (choice == "number") {
            var query = $("#query").val();
            if (query == "")
                return;
            var tmpList = ALLCONTACTS;
            var incomplete = [];
            ALLCONTACTS = [];
            for (var i = 0; i < tmpList.length; ++i) {
                if (tmpList[i].phones == null)
                    continue;
                for (var j = 0; j < tmpList[i].phones.length; ++j) {
                    if (tmpList[i].phones[j].phoneNumber == query || tmpList[i].phones[j].phoneNumber.toLowerCase() == query.toLowerCase()) {
                        ALLCONTACTS.push(tmpList[i]);
                        break;
                    }
                    else if (tmpList[i].phones[j].phoneNumber.toLowerCase().search(query.toLowerCase()) >= 0) {
                        incomplete.push(tmpList[i]);
                        break;
                    }
                }
            }
            ALLCONTACTS = ALLCONTACTS.concat(incomplete);
            // Sada su tu učitani svi kontakti koji zadovoljavaju uvjet
            // Gumb za povratak u normalu
            $("#deactivate").removeClass("hidden");
            $("#deactivate").on("click", function () {
                ALLCONTACTS = tmpList;
                currentPage = 0;
                $("#showHideFilter").trigger("click");
                startPage();
            });

            currentPage = 0;
            startPage();
        }


    });
});