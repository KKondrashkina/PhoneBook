<template>
    <div id="app">
        <div>
            <b-navbar toggleable="lg" type="dark" variant="info">
                <b-navbar-brand href="#">PhoneBook</b-navbar-brand>

                <b-navbar-toggle target="nav-collapse"></b-navbar-toggle>

                <b-collapse id="nav-collapse" is-nav>
                    <b-navbar-nav class="ml-auto">
                        <b-nav-form>
                            <b-form-input v-model="searchString" size="sm" class="mr-2" placeholder="Search"></b-form-input>
                            <b-button @click="search" size="sm" class="my-2 mr-2">Search</b-button>
                            <b-button @click="reset" size="sm">Reset</b-button>
                        </b-nav-form>
                    </b-navbar-nav>
                </b-collapse>
            </b-navbar>
        </div>

        <b-container fluid class="my-3">
            <router-view />
        </b-container>
    </div>
</template>

<script>
    export default {
        data() {
            return {
                searchString: ""
            }
        },
        methods: {
            search() {
                this.$store.state.searchString = this.searchString;
                this.$store.dispatch("getContacts");
            },
            reset() {
                this.searchString = "";
                this.search();
            }
        }
    }
</script>
