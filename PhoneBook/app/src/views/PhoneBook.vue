<template>
    <div class="phoneBook">
        <b-row>
            <b-col md="3">
                <div>
                    <label for="input-name">Name:</label>
                    <b-form-input id="input-name" placeholder="Name" v-model="contactName" :state="isValidName"></b-form-input>
                    <b-form-invalid-feedback id="input-name-feedback">
                        {{nameInvalidMessage}}
                    </b-form-invalid-feedback>

                    <label for="input-last-name" class="mt-3">Last name:</label>
                    <b-form-input id="input-last-name" placeholder="Last name" v-model="contactLastName" :state="isValidLastName"></b-form-input>
                    <b-form-invalid-feedback id="input-last-name-feedback">
                        {{lastNameInvalidMessage}}
                    </b-form-invalid-feedback>

                    <label for="input-phone-number" class="mt-3">Phone number:</label>
                    <b-form-input id="input-phone-number" placeholder="Phone number" v-model="contactPhoneNumber" :state="isValidPhoneNumber"></b-form-input>
                    <b-form-invalid-feedback id="input-phone-number-feedback">
                        {{phoneNumberInvalidMessage}}
                    </b-form-invalid-feedback>

                    <b-row align-h="end" class="mb-3 mt-3">
                        <b-col cols="auto">
                            <b-button variant="secondary" class="form-button">Cancel</b-button>
                            <b-button @click="addContact" variant="primary" class="ml-2 form-button">Add</b-button>
                        </b-col>
                    </b-row>
                </div>
            </b-col>
            <b-col>
                <b-modal id="deleteSelectedModal" title="Confirm deletion" v-model="isShowContactsModal">
                    Are you sure you want to delete these {{selected.length}} contacts?
                    <template v-slot:modal-footer>
                        <b-button size="sm" @click="isShowContactsModal = false">
                            Cancel
                        </b-button>
                        <b-button size="sm" variant="info" @click="deleteSelected">
                            Delete
                        </b-button>
                    </template>
                </b-modal>

                <b-row align-h="between">
                    <b-col>
                        <b-button size="sm" @click="selectAllRows" class="mr-2 mb-2">Select all</b-button>
                        <b-button size="sm" @click="clearSelected" class="mr-2 mb-2">Clear selected</b-button>
                        <b-button v-b-modal.deleteSelectedModal size="sm" variant="outline-danger" :disabled="selected.length === 0" class="mb-2">Delete selected</b-button>
                    </b-col>
                    <b-col cols="auto">
                        <b-button href="/api/DownloadFile" size="sm" variant="outline-primary">Export</b-button>
                    </b-col>
                </b-row>

                <b-row align-h="between">
                    <b-col md="4" class="my-1">
                        <b-form-group label="Per page"
                                      label-cols-md="4"
                                      label-cols-lg="3"
                                      label-size="sm"
                                      label-for="perPageSelect"
                                      class="mb-0">
                            <b-form-select v-model="perPage"
                                           id="perPageSelect"
                                           size="sm"
                                           :options="pageOptions"></b-form-select>
                        </b-form-group>
                    </b-col>

                    <b-col md="4" class="my-1">
                        <b-pagination v-model="currentPage"
                                      :total-rows="totalRows"
                                      :per-page="perPage"
                                      align="fill"
                                      size="sm"
                                      class="my-0"></b-pagination>
                    </b-col>
                </b-row>

                <b-table striped hover id="contacts-table"
                         :items="contacts"
                         :fields="fields"
                         :current-page="currentPage"
                         :per-page="perPage"
                         ref="selectableTable"
                         selectable
                         select-mode="multi"
                         @row-selected="onRowSelected">
                    <template v-slot:cell(number)="row">
                        <div>{{(row.index + 1) + (currentPage - 1) * perPage}}</div>
                    </template>

                    <template v-slot:cell(isSelected)="{ rowSelected }">
                        <template v-if="rowSelected">
                            <span aria-hidden="true">&check;</span>
                            <span class="sr-only">Selected</span>
                        </template>
                        <template v-else>
                            <span aria-hidden="true">&nbsp;</span>
                            <span class="sr-only">Not selected</span>
                        </template>
                    </template>

                    <template v-slot:cell(button)="row">
                        <b-button v-b-modal="'deleteContactModal' + row.index" size="sm" variant="outline-danger">X</b-button>
                        <b-modal :id="'deleteContactModal' + row.index" title="Confirm deletion">
                            Are you sure you want to delete <strong>{{row.item.name}} {{row.item.lastName}}</strong>?
                            <template v-slot:modal-footer>
                                <b-button size="sm" @click="hideModal('deleteContactModal' + row.index)">
                                    Cancel
                                </b-button>
                                <b-button size="sm" variant="info" @click="deleteContact(row.item.id, 'deleteContactModal' + row.index)">
                                    Delete
                                </b-button>
                            </template>
                        </b-modal>
                    </template>
                </b-table>
            </b-col>
        </b-row>
    </div>
</template>

<script>
    export default {
        name: "PhoneBook",
        data() {
            return {
                fields:
                    [
                        {
                            key: "isSelected",
                            label: ""
                        },
                        {
                            key: "number",
                            label: ""
                        },
                        {
                            key: "name",
                            label: "Name",
                            sortable: true
                        },
                        {
                            key: "lastName",
                            label: "Last Name",
                            sortable: true
                        },
                        {
                            key: "phoneNumber",
                            label: "Phone Number"
                        },
                        {
                            key: "button",
                            label: ""
                        }
                    ],
                selected: [],

                currentPage: 1,
                perPage: 5,
                pageOptions: [5, 10, 15],
                totalRows: 1,

                contactName: "",
                contactLastName: "",
                contactPhoneNumber: "",

                isValidName: null,
                isValidLastName: null,
                isValidPhoneNumber: null,

                nameInvalidMessage: "",
                lastNameInvalidMessage: "",
                phoneNumberInvalidMessage: "",

                isShowContactsModal: false,

                searchString: ""
            };
        },
        methods: {
            addContact() {
                if (this.IsInvalidContact() || this.IsContactExist()) {
                    return
                } else {
                    this.isValidName = null;
                    this.isValidLastName = null;
                    this.isValidPhoneNumber = null;
                }

                var contact = {
                    lastName: this.contactLastName,
                    name: this.contactName,
                    phoneNumber: this.contactPhoneNumber
                };

                this.$store.dispatch("addContact", contact);
                this.$store.dispatch("getContacts");

                this.contactLastName = "";
                this.contactName = "";
                this.contactPhoneNumber = "";
            },
            IsInvalidContact() {
                var isInvalid = false;

                if (this.contactName === "") {
                    this.isValidName = false;
                    this.nameInvalidMessage = "Name is required";

                    isInvalid = true;
                } else {
                    this.isValidName = true;
                }

                if (this.contactLastName === "") {
                    this.isValidLastName = false;
                    this.lastNameInvalidMessage = "Last Name is required";

                    isInvalid = true;
                } else {
                    this.isValidLastName = true;
                }

                if (this.contactPhoneNumber === "") {
                    this.isValidPhoneNumber = false;
                    this.phoneNumberInvalidMessage = "Phone Number is required";

                    isInvalid = true;
                } else if (this.contactPhoneNumber.search(/[^0-9-+()]/) !== -1) {
                    this.isValidPhoneNumber = false;
                    this.phoneNumberInvalidMessage = "Only numbers and ( ) + - symbols allowed";

                    isInvalid = true;
                }
                else {
                    this.isValidPhoneNumber = true;
                }

                return isInvalid;
            },
            IsContactExist() {
                if (this.contacts.map(c => c.phoneNumber).indexOf(this.contactPhoneNumber) >= 0) {
                    this.isValidPhoneNumber = false;
                    this.phoneNumberInvalidMessage = "Contact with the same number already exists";

                    return true;
                }

                return false;
            },
            deleteContact(id, modalId) {
                this.$store.dispatch("deleteContact", id);
                this.$store.dispatch("getContacts");
                this.$bvModal.hide(modalId);
            },
            deleteSelected() {
                this.$store.dispatch("deleteContacts", this.selected.map(s => s.id));
                this.isShowContactsModal = false;
            },
            onRowSelected(items) {
                this.selected = items
            },
            selectAllRows() {
                this.$refs.selectableTable.selectAllRows()
            },
            clearSelected() {
                this.$refs.selectableTable.clearSelected()
            },
            hideModal(modalId) {
                this.$bvModal.hide(modalId);
            }
        },
        created() {
            this.$store.dispatch("getContacts");
        },
        computed: {
            contacts() {
                return this.$store.state.contacts;
            },
            isChange() {
                return this.$store.state.isChange;
            }
        },
        watch: {
            isChange() {
                this.$store.dispatch("getContacts");
                this.$store.state.isChange = false;
            },
            contacts() {
                this.totalRows = this.contacts.length
            }
        }
    };
</script>

<style>
    .form-button {
        min-width: 75px
    }
</style>
