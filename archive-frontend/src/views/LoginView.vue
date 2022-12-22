<template>
    <v-form>
        <div class="d-flex flex-column">
            <v-card :loading="loading" class="ml-auto mr-auto mt-5 px-5" color="">
                <v-card-title>Login</v-card-title>
                <v-text-field v-model="loginModel.LoginName" clearable outlined label="Login Name"
                    placeholder="Login name">
                </v-text-field>
                <v-text-field v-model="loginModel.Password" :type="showPassword ? 'password' : ''" outlined
                    label="Password" placeholder="Placeholder" :append-icon="showPassword ? 'mdi-eye' : 'mdi-eye-off'"
                    class="ml-auto mr-auto mt-5" @click:append="togglePassword">
                </v-text-field>
                <v-checkbox v-model="loginModel.RememberMe" label="Remember me">Remember me</v-checkbox>
                <v-card-actions>
                    <v-btn color="primary" class="" @click="login()">Login</v-btn>
                    <router-link to="/register" style="text-decoration: none;" class="ml-5">
                        <v-btn color="primary">Create account</v-btn>
                    </router-link>
                </v-card-actions>


            </v-card>

        </div>
    </v-form>
</template>

<script lang="ts">import { address } from '@/store/environment';
import { defineComponent } from 'vue';
import { useGlobalStore } from '../store/global';
import { storeToRefs } from 'pinia';

export default defineComponent({


    setup() {
        const { token } = storeToRefs(useGlobalStore())
        let loginModel: any = { LoginName: '', Password: '', RememberMe: false }

        return {
            loginModel,
            token
        }
    },
    data() {
        return {
            loading: false,
            showPassword: false,

        }
    },

    methods: {
        togglePassword() {
            this.showPassword = !this.showPassword;
        }
        ,
        login() {
            const form = new FormData();
            Object.keys(this.loginModel).forEach((key) => form.append(key, this.loginModel[key]));

            return fetch(address() + "/token/get", {
                body: form,
                method: "POST",
            }).then(response => {
                console.log(response);
                if (response.status == 200) {
                    response.text().then(token => this.token = token);
                }
                this.$router.push("/")
            }).catch(error => {
            })

        }

    },

})
</script>
<style lang="">
    
</style>