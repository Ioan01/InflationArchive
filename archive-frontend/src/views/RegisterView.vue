<template>
    <v-form>
        <div class="d-flex flex-column">
            <v-card :loading="loading" class="ml-auto mr-auto mt-5 px-5" color="">
                <v-card-title>Register</v-card-title>
                <v-text-field v-model="registerModel.Username" clearable outlined label="Username"
                    placeholder="Username">
                </v-text-field>
                <v-text-field v-model="registerModel.Email" clearable outlined label="Email" placeholder="Email">
                </v-text-field>
                <v-text-field v-model="registerModel.Password" :type="showPassword ? 'password' : ''" outlined
                    label="Password" placeholder="Placeholder" :append-icon="showPassword ? 'mdi-eye' : 'mdi-eye-off'"
                    class="ml-auto mr-auto mt-5" @click:append="togglePassword">
                </v-text-field>
                <v-text-field v-model="registerModel.ConfirmPassword" :type="showPassword ? 'confirmPassword' : ''"
                    outlined label="Confirm Password" placeholder="Placeholder"
                    :append-icon="showPassword ? 'mdi-eye' : 'mdi-eye-off'" class="ml-auto mr-auto mt-5"
                    @click:append="togglePassword">
                </v-text-field>
                <v-card-actions>
                    <v-btn color="primary" class="" @click="register()">Register</v-btn>
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
        let registerModel: any = { Username: '', Email: '', Password: '', ConfirmPassword: '' }

        return {
            registerModel,
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
        register() {
            const form = new FormData();
            Object.keys(this.registerModel).forEach((key) => form.append(key, this.registerModel[key] as string));

            return fetch(address() + "/Account/Register", {
                body: form,
                method: "POST",
            }).catch(error => {
            })

        }

    },

})
</script>
<style lang="">
    
</style>