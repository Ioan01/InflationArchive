
<template lang="html">
    <div>
        <div class="container d-flex">
            <v-card class="mx-auto">
                <v-img :src="item?.imageUri"></v-img>

                <v-card-title>
                    {{ item?.name }}
                </v-card-title>
                {{ item?.pricePerUnit }} LEI / {{ item?.unit }}
                <v-card-subtitle>
                </v-card-subtitle>
                <v-row class="justify-center mx-auto pa-3">
                    {{ item?.category }}
                </v-row>
            </v-card>
            <v-container fluid>
                <v-sheet class="v-sheet--offset mx-auto" elevation="12">
                    <v-sparkline stroke-linecap="round" label-size="5" type="trend" line-width="1" radius="2" smooth
                        :value="values" :labels="labels" auto-draw></v-sparkline>
                </v-sheet>
            </v-container>
        </div>
    </div>
</template>
<script lang="ts">
import { ProductModel } from '@/models/ProductModel';
import { address } from '@/store/environment';
import axios from 'axios';
import Axios from 'axios';
import { Ref, defineComponent, ref } from 'vue';
import { useRoute } from 'vue-router/types/composables';


export default defineComponent({

    beforeMount() {


        this.fetchProductData(this.$route.params.productId)
    },

    setup() {

        const item: Ref<ProductModel | undefined> = ref()



        return {
            item
        }
    },

    methods: {
        async fetchProductData(id?: String) {
            const response = await axios.get<ProductModel>(
                address() + '/product/getProduct?productId=' + id
            );
            console.log(response.data);
            this.item = response.data

            this.item

        }
    }
    ,
    computed: {
        values(): number[] | undefined {
            return this.item?.productPrices.map(price => price.price)
        },
        labels(): string[] | undefined {
            return this.item?.productPrices.map(price => price.date.substring(0, 10) + '\n' + price.price.toString())
        }
    }


})
</script>
<style lang="">
    
</style>