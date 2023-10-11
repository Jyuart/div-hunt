<script lang="ts">
	import { browser } from '$app/environment';
	import * as signalR from '@microsoft/signalr';
	import type { Coordinates } from '../types/coordinates';

	let connection: signalR.HubConnection;

	// current device coordinates
	let alpha: number;
	let beta: number;

	// connected device coordinates
	let mobileHorizontal: number;
	let mobileVertical: number;

	let catLeft = 50;
	let catTop = 50;

	let fishLeft = 68;
	let fishTop = 20;

	const nodeHubUrl = `${process.env.VITE_HUB_APP_URL}/hub`;
	console.log(nodeHubUrl);
	const hubUrl = `${import.meta.env.VITE_HUB_APP_URL}/hub`;
	console.log(hubUrl);

	// an easy way to skip SSR and run code in browser
	if (browser) {
		const mobile = isMobile();

		const nodeHubUrl = `${process.env.VITE_HUB_APP_URL}/hub`;
		console.log(nodeHubUrl);
		const hubUrl = `${import.meta.env.VITE_HUB_APP_URL}/hub`;
		console.log(hubUrl);
		console.log("hello");
		console.log(hubUrl);
		console.log(nodeHubUrl);

		connection = new signalR.HubConnectionBuilder().withUrl(hubUrl).build();
		connection.on('SendCoordinates', setMobileCoordinates);
		connection.start();

		// no need to have this event listener for a desktop
		if (mobile) {
			window.addEventListener('deviceorientation', handleOrientation, true);
		}

		// can be less to improve responsiveness
		setInterval(() => {
			checkOverlapping();
		}, 250);
	}

	function sendCoordinatesToHub(alpha: number, beta: number) {
		connection.invoke('ReceiveCoordinates', { horizontal: alpha, vertical: beta });
	}

	function setMobileCoordinates(coordinates: Coordinates) {
		mobileHorizontal = coordinates.horizontal;
		mobileVertical = coordinates.vertical;
		calculateHorizontalPosition(coordinates.horizontal);
		calculateVerticalPosition(coordinates.vertical);
	}

	// https://stackoverflow.com/questions/5731863/mapping-a-numeric-range-onto-another
	function calculateHorizontalPosition(horizontal: number) {
		// let leftCoordinates = 0 + ((100 - 0) / (60 - 120)) * (horizontal - 120);
		let left = mapRanges(0, 100, 60, 120, horizontal);

		if (left < 0) {
			left = 0;
		} else if (left > 100) {
			left = 100;
		}

		catLeft = left;
	}

	function calculateVerticalPosition(vertical: number) {
		//divTop = 0 + ((100 - 0) / (-30 - 30)) * (vertical - 30);
		catTop = mapRanges(0, 100, -30, 30, vertical);
	}

	function handleOrientation(event: DeviceOrientationEvent) {
		alpha = toTwoDecimalPlaces(event.alpha!);
		beta = toTwoDecimalPlaces(event.beta!);
		sendCoordinatesToHub(alpha, beta);
	}

	function mapRanges(
		output_start: number,
		output_end: number,
		input_start: number,
		input_end: number,
		input: number
	) {
		return (
			output_start +
			((output_end - output_start) / (input_end - input_start)) * (input - input_start)
		);
	}

	function toTwoDecimalPlaces(num: number) {
		return Math.round(num * 100) / 100;
	}

	function isMobile() {
		return /Mobi/i.test(window.navigator.userAgent);
	}

	function checkOverlapping() {
		let hero = document.getElementsByClassName('hero')[0];
		let victim = document.getElementsByClassName('victim')[0];
		let heroRect = hero.getBoundingClientRect();
		let victimRect = victim.getBoundingClientRect();
		var overlap = !(
			heroRect.right < victimRect.left ||
			heroRect.left > victimRect.right ||
			heroRect.bottom < victimRect.top ||
			heroRect.top > victimRect.bottom
		);

		if (overlap) {
			fishLeft = Math.random() * 80 + 10;
			fishTop = Math.random() * 80 + 10;
		}
	}
</script>

<section style="position: absolute;">
	<h3>Legend</h3>
	<p>My horizontal: {alpha}</p>
	<p>My vertical: {beta}</p>

	<p>Mobile horizontal: {mobileHorizontal}</p>
	<p>Mobile vertical: {mobileVertical}</p>
</section>

<section class="main">
	<div class="cat" style="left: {catLeft}%; top: {catTop}%" />
	<div
		class="fish"
		style="left: {fishLeft}%; top: {fishTop}%; rotate: {Math.floor(Math.random() * 360)}deg"
	/>
</section>

<style>
	.main {
		height: 100vh;
		width: 100vw;
	}

	div {
		position: absolute;
		background-size: contain;
	}

	.cat {
		width: 200px;
		height: 200px;
		background-image: url('/cat.png');
		transform: translate(-100%, -50%);
	}

	.fish {
		width: 100px;
		height: 100px;
		background-image: url('/fish.png');
	}
</style>
