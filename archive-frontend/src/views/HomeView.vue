import { useGlobalStore } from '../store/global';
<template lang="html">
    <div class="container" style="max-width: 90%">
        <div v-if="loggedIn === true">
            <h2>Here are your favorite products:</h2>

        </div>
        <h2 class="text-center mt-5">We have the following products you can browse:</h2>
        <h3 class="text-center mt-5">You can filter the products using the following criteria:</h3>
        <div class="d-flex mt-5">
            <v-btn color="primary">store</v-btn>
            <v-spacer></v-spacer>
            <v-btn color="primary">Category</v-btn>
            <v-spacer></v-spacer>
            <v-btn color="primary">Price Ascending</v-btn>
            <v-spacer></v-spacer>
            <v-btn color="primary">Price Descending</v-btn>
        </div>
        <div class="d-flex flex-wrap mx-auto">
            <li v-for="product in products" :key="product.id">
                <v-card class="mt-5" max-width="344">
                    <v-img src={{ product.imageUri }} height="200px"></v-img>

                    <v-card-title>
                        {{ product.name}}
                    </v-card-title>

                    <v-card-subtitle>
                        Do we really need this buddy?
                    </v-card-subtitle>
                    <v-row class="justify-center mx-auto pa-3">
                        <v-btn
                            color="primary" >
                            Product details
                        </v-btn>
                    </v-row>
                </v-card>
            </li>
            
        <v-card class=" ml-5 mt-5" max-width="344">
            <v-img src="https://cdn.vuetifyjs.com/images/cards/sunshine.jpg" height="200px"></v-img>

            <v-card-title>
                Product name lalala
            </v-card-title>

            <v-card-subtitle>
                Do we really need this buddy?
            </v-card-subtitle>
            <v-row class="justify-center mx-auto pa-3">
                <v-btn
                    color="primary" >
                    Product details
                </v-btn>
            </v-row>
        </v-card>
        <v-card class="ml-5 mt-5" max-width="344">
            <v-img src="https://cdn.vuetifyjs.com/images/cards/sunshine.jpg" height="200px"></v-img>

            <v-card-title>
                Product name lalala
            </v-card-title>

            <v-card-subtitle>
                Do we really need this buddy?
            </v-card-subtitle>
            <v-row class="justify-center mx-auto pa-3">
                <v-btn
                    color="primary" style="margin-bottom: 20px">
                    Product details
                </v-btn>
            </v-row>
        </v-card>
        <v-card class="ml-5 mt-5" max-width="344">
            <v-img src="https://cdn.vuetifyjs.com/images/cards/sunshine.jpg" height="200px"></v-img>

            <v-card-title>
                Product name lalala
            </v-card-title>

            <v-card-subtitle>
                Do we really need this buddy?
            </v-card-subtitle>
            <v-row class="justify-center mx-auto pa-3">
                <v-btn
                    color="primary" >
                    Product details
                </v-btn>
            </v-row>
        </v-card>
        </div>
        
    </div>
</template>
<script lang="ts">import { useGlobalStore } from '@/store/global';
import { ProductModel } from '@/models/ProductModel';
import axios from 'axios';
import { defineComponent, ref} from 'vue';
import { address } from '@/store/environment';

export default defineComponent({
  setup() {
    const { loggedIn } = useGlobalStore();
    const products = ref<ProductModel[]>([]);

    async function fetchProducts() {
      try {
        const response = await axios.get<ProductModel[]>(
          address() + '/product/getProducts'
        );
        products.value = response.data;
      } catch (error) {
        console.error(error);
      }
    }

    fetchProducts();

    return {
      loggedIn,
      products,
    };
  },
});
</script>
<style lang="">
    
</style>